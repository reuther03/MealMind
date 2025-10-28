﻿using System.Text.Json.Serialization;
using MealMind.Modules.Nutrition.Application.Dtos;
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

    [JsonPropertyName("nutriments")]
    public NutrimentsDto Nutriments { get; init; } = null!;

    public static Food MapToFood(OpenFoodFactsDto dto)
    {
        return Food.Create(
            new Name(dto.ProductName),
            new NutritionPer100G(
                dto.Nutriments.EnergyKcal100G,
                dto.Nutriments.Proteins100G,
                dto.Nutriments.Fat100G,
                dto.Nutriments.Carbohydrates100G,
                dto.Nutriments.Sugars100G,
                dto.Nutriments.SaturatedFat100G,
                dto.Nutriments.Fiber100G,
                dto.Nutriments.Sodium100G,
                dto.Nutriments.Salt100G,
                dto.Nutriments.Cholesterol100G),
            Source.ExternalApi,
            dto.Code,
            dto.ImageUrl,
            dto.Brand);
    }

    public static FoodDto MapFoodDto(OpenFoodFactsDto dto)
    {
        return new FoodDto
        {
            Id = null,
            Name = dto.ProductName,
            Barcode = dto.Code,
            Brand = dto.Brand,
            ImageUrl = dto.ImageUrl,
            NutritionPer100G = new NutrimentsPer100GDto
            {
                Calories = dto.Nutriments.EnergyKcal100G,
                Protein = dto.Nutriments.Proteins100G,
                Fat = dto.Nutriments.Fat100G,
                Carbohydrates = dto.Nutriments.Carbohydrates100G,
                Sugar = dto.Nutriments.Sugars100G,
                SaturatedFat = dto.Nutriments.SaturatedFat100G,
                Fiber = dto.Nutriments.Fiber100G,
                Sodium = dto.Nutriments.Sodium100G,
                Salt = dto.Nutriments.Salt100G,
                Cholesterol = dto.Nutriments.Cholesterol100G
            },
            CreatedAt = DateTime.UtcNow,
            FoodSource = nameof(Source.ExternalApi)
        };
    }
}