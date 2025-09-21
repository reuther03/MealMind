using MealMind.Modules.Nutrition.Domain.Food;

namespace MealMind.Modules.Nutrition.Application.Features.Queries.Dtos;

public class NutrimentsPer100gDto
{
    public decimal Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbohydrates { get; init; }
    public decimal Fat { get; init; }
    public decimal? Fiber { get; init; }
    public decimal? Sugar { get; init; }
    public decimal? SaturatedFat { get; init; }
    public decimal? Sodium { get; init; }

    public static NutrimentsPer100gDto AsDto(NutritionPer100G nutrition)
    {
        return new NutrimentsPer100gDto
        {
            Calories = nutrition.Calories,
            Protein = nutrition.Protein,
            Carbohydrates = nutrition.Carbohydrates,
            Fat = nutrition.Fat,
            Fiber = nutrition.Fiber,
            Sugar = nutrition.Sugar,
            SaturatedFat = nutrition.SaturatedFat,
            Sodium = nutrition.Sodium
        };
    }
}