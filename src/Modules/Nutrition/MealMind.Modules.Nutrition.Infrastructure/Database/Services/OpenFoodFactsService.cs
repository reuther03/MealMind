using System.Text.Json;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Domain.Food;
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
        _httpClient.BaseAddress = new Uri("https://world.openfoodfacts.net");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MealMind/1.0");
        _logger = logger;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<List<Food>> SearchFoodByNameAsync(string name, int limit = 20, CancellationToken cancellationToken = default)
    {
        var url = $"/cgi/search.pl?search_terms={Uri.EscapeDataString(name)}&search_simple=1&action=process&json=1&page_size={limit}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch data from OpenFoodFacts API. Status Code: {StatusCode}", response.StatusCode);
            return [];
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var searchResult = JsonSerializer.Deserialize<List<OpenFoodFactsDto>>(content, _jsonSerializerOptions);

        if (searchResult == null)
        {
            _logger.LogInformation("No products found for search term: {SearchTerm}", name);
            return [];
        }

        var foods = searchResult.Select(x => x.MapToFood(x)).ToList();
        return foods;
    }
}