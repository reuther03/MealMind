using System.Text.Json.Serialization;

namespace MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts;

public class NutrimentsDto
{
    [JsonPropertyName("energy-kcal_100g")]
    public decimal EnergyKcal100g { get; init; }
    [JsonPropertyName("proteins_100g")]
    public decimal Proteins100g { get; init; }
    [JsonPropertyName("fat_100g")]
    public decimal Fat100g { get; init; }
    [JsonPropertyName("carbohydrates_100g")]
    public decimal Carbohydrates100g { get; init; }
    [JsonPropertyName("sugars_100g")]
    public decimal? Sugars100g { get; init; }
    [JsonPropertyName("saturated-fat_100g")]
    public decimal? SaturatedFat100g { get; init; }
    [JsonPropertyName("fiber_100g")]
    public decimal? Fiber100g { get; init; }
    [JsonPropertyName("sodium_100g")]
    public decimal? Sodium100g { get; init; }
    [JsonPropertyName("cholesterol_100g")]
    public decimal? Cholesterol100g { get; init; }
}