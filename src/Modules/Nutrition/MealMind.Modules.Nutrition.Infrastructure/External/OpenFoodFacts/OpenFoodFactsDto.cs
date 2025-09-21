using System.Text.Json.Serialization;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts;

public class OpenFoodFactsDto
{
    [JsonPropertyName("product_name")]
    public string ProductName { get; init; } = null!;

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("brands")]
    public string Brand { get; init; } = null!;

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; init; }

    public NutrimentsDto Nutriments { get; init; } = null!;

    public Food MapToFood(OpenFoodFactsDto dto)
    {
        return Food.Create(
            new Name(dto.ProductName),
            new NutritionPer100G(
                dto.Nutriments.EnergyKcal100g,
                dto.Nutriments.Proteins100g,
                dto.Nutriments.Fat100g,
                dto.Nutriments.Carbohydrates100g,
                dto.Nutriments.Sugars100g,
                dto.Nutriments.SaturatedFat100g,
                dto.Nutriments.Fiber100g,
                dto.Nutriments.Sodium100g,
                dto.Nutriments.Cholesterol100g),
            Source.ExternalApi,
            dto.ImageUrl,
            dto.Code,
            dto.Brand);
    }
}