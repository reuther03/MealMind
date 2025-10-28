using System.Text.Json;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Application.Dtos;
using MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Services;

public class OpenFoodFactsService : IOpenFoodFactsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenFoodFactsService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public OpenFoodFactsService(HttpClient httpClient, ILogger<OpenFoodFactsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    public async Task<List<FoodDto>> SearchFoodByNameAsync(string name, int pageSize = 10, int page = 1, CancellationToken cancellationToken = default)
    {
        var url = $"/cgi/search.pl?search_terms={Uri.EscapeDataString(name)}&search_simple=1&action=process&json=1&page_size={pageSize}&page={page}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch data from OpenFoodFacts API. Status Code: {StatusCode}", response.StatusCode);
            return [];
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var searchResult = JsonSerializer.Deserialize<OpenFoodFactsResponseDto>(content, _jsonSerializerOptions);

        if (searchResult is null || searchResult.Products is null || searchResult.Products.Count == 0)
        {
            _logger.LogInformation("No products found for search term: {SearchTerm}", name);
            return [];
        }

        var foods = searchResult.Products
            .Select(OpenFoodFactsDto.MapFoodDto)
            .ToList();

        return foods;
    }

    public async Task<FoodDto> GetFoodByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var url = $"/api/v2/product/{barcode}.json";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch data from OpenFoodFacts API. Status Code: {StatusCode}", response.StatusCode);
            return null!;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var searchResult = JsonSerializer.Deserialize<OpenFoodFactsBarcodeResponseDto>(content, _jsonSerializerOptions);

        if (searchResult?.Product is not null)
            return OpenFoodFactsDto.MapFoodDto(searchResult.Product);

        _logger.LogInformation("No products found for search term: {SearchTerm}", barcode);
        return null!;
    }

    public Task<List<FoodDto>> SearchFoodByNameWithoutDuplicatesAsync(string name, int pageSize, int page, List<FoodDto> existingFoods,
        CancellationToken cancellationToken = default)
    {
        return SearchFoodByNameAsync(name, pageSize, page, cancellationToken)
            .ContinueWith(task =>
            {
                var newFoods = task.Result;
                var filteredFoods = newFoods
                    .Where(f => !existingFoods.Any(ef => ef.Name.Equals(f.Name, StringComparison.OrdinalIgnoreCase) && ef.Brand == f.Brand)).ToList();
                return filteredFoods;
            }, cancellationToken);
    }
}