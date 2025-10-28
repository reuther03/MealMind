using System.Text.Json.Serialization;
using MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts.Converters;

namespace MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts;

public class OpenFoodFactsResponseDto
{
    // Total number of products found
    [JsonPropertyName("count")]
    public int ProductCount { get; init; }

    [JsonPropertyName("page")]
    [JsonConverter(typeof(FlexibleNumberConverter))]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; init; }

    [JsonPropertyName("products")]
    public List<OpenFoodFactsDto>? Products { get; init; }
}

public class OpenFoodFactsBarcodeResponseDto
{
    // Total number of products found
    [JsonPropertyName("count")]
    public int ProductCount { get; init; }

    [JsonPropertyName("page")]
    [JsonConverter(typeof(FlexibleNumberConverter))]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; init; }

    [JsonPropertyName("product")]
    public OpenFoodFactsDto? Product { get; init; }
}