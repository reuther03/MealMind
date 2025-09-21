using System.Text.Json.Serialization;

namespace MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts;

public class OpenFoodFactsResponseDto
{
    [JsonPropertyName("count")]
    public int Count { get; init; } // Total number of products found

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; init; }

    [JsonPropertyName("products")]
    public List<OpenFoodFactsDto>? Products { get; init; }
}