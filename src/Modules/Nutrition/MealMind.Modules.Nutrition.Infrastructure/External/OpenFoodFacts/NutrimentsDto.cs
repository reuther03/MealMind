using System.Text.Json.Serialization;

namespace MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts;

public class NutrimentsDto
{
    [JsonPropertyName("energy-kcal_100g")]
    public decimal EnergyKcal100G { get; init; }

    [JsonPropertyName("proteins_100g")]
    public decimal Proteins100G { get; init; }

    [JsonPropertyName("fat_100g")]
    public decimal Fat100G { get; init; }

    [JsonPropertyName("carbohydrates_100g")]
    public decimal Carbohydrates100G { get; init; }

    [JsonPropertyName("sugars_100g")]
    public decimal? Sugars100G { get; init; }

    [JsonPropertyName("saturated-fat_100g")]
    public decimal? SaturatedFat100G { get; init; }

    [JsonPropertyName("fiber_100g")]
    public decimal? Fiber100G { get; init; }

    [JsonPropertyName("sodium_100g")]
    public decimal? Sodium100G { get; init; }

    [JsonPropertyName("salt_100g")]
    public decimal? Salt100G { get; init; }

    [JsonPropertyName("cholesterol_100g")]
    public decimal? Cholesterol100G { get; init; }

    public NutrimentsDto(decimal energyKcal100G, decimal proteins100G, decimal fat100G, decimal carbohydrates100G, decimal? sugars100G,
        decimal? saturatedFat100G, decimal? fiber100G, decimal? sodium100G, decimal? cholesterol100G)
    {
        EnergyKcal100G = energyKcal100G;
        Proteins100G = proteins100G;
        Fat100G = fat100G;
        Carbohydrates100G = carbohydrates100G;
        Sugars100G = sugars100G;
        SaturatedFat100G = saturatedFat100G;
        Fiber100G = fiber100G;
        Sodium100G = sodium100G;
        Cholesterol100G = cholesterol100G;
    }
}