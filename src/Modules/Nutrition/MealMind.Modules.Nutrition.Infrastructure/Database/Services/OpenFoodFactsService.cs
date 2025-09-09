using System.Text.Json;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Domain.Food;
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

    public Task<List<Food>> SearchFoodByNameAsync(string name, int limit = 20, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}