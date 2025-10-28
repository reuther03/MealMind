using MealMind.Modules.Nutrition.Domain.Food;

namespace MealMind.Modules.Nutrition.Application.Dtos;

public class NutrimentsPer100GDto
{
    public decimal Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbohydrates { get; init; }
    public decimal Fat { get; init; }
    public decimal? Fiber { get; init; }
    public decimal? Sugar { get; init; }
    public decimal? SaturatedFat { get; init; }
    public decimal? Sodium { get; init; }
    public decimal? Salt { get; init; }
    public decimal? Cholesterol { get; init; }


    public static NutrimentsPer100GDto AsDto(NutritionPer100G nutrition)
    {
        return new NutrimentsPer100GDto
        {
            Calories = nutrition.Calories,
            Protein = nutrition.Protein,
            Fat = nutrition.Fat,
            Carbohydrates = nutrition.Carbohydrates,
            Fiber = nutrition.Fiber,
            Sugar = nutrition.Sugar,
            SaturatedFat = nutrition.SaturatedFat,
            Sodium = nutrition.Sodium,
            Salt = nutrition.Salt,
            Cholesterol = nutrition.Cholesterol
        };
    }

    public NutritionPer100G ToEntity(NutrimentsPer100GDto dto)
    {
        return new NutritionPer100G(
            dto.Calories,      // calories
            dto.Protein,       // protein
            dto.Fat,           // fat
            dto.Carbohydrates, // carbohydrates
            dto.Salt,          // salt (5th parameter!)
            dto.Sugar,         // sugar
            dto.SaturatedFat,  // saturatedFat
            dto.Fiber,         // fiber
            dto.Sodium,        // sodium
            dto.Cholesterol    // cholesterol
        );
    }
}