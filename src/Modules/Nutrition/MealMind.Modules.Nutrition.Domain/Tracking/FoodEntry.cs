using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public class FoodEntry : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public Name FoodName { get; private set; }
    public Name? FoodBrand { get; private set; }
    public decimal QuantityInGrams { get; private set; }
    public decimal TotalCalories { get; private set; }
    public decimal TotalProteins { get; private set; }
    public decimal TotalCarbohydrates { get; private set; }
    public decimal? TotalSugars { get; private set; }
    public decimal TotalFats { get; private set; }
    public decimal? TotalSaturatedFats { get; private set; }
    public decimal? TotalFiber { get; private set; }
    public decimal? TotalSodium { get; private set; }
    public decimal? TotalSalt { get; private set; }
    public decimal? TotalCholesterol { get; private set; }

    private FoodEntry()
    {
    }

    private FoodEntry(FoodId foodId, Name foodName, Name? foodBrand, decimal quantityInGrams, decimal totalCalories, decimal totalProteins,
        decimal totalCarbohydrates, decimal? totalSugars, decimal totalFats, decimal? totalSaturatedFats, decimal? totalFiber, decimal? totalSodium,
        decimal? totalSalt, decimal? totalCholesterol)
    {
        FoodId = foodId;
        FoodName = foodName;
        FoodBrand = foodBrand;
        QuantityInGrams = quantityInGrams;
        TotalCalories = totalCalories;
        TotalProteins = totalProteins;
        TotalCarbohydrates = totalCarbohydrates;
        TotalSugars = totalSugars;
        TotalFats = totalFats;
        TotalSaturatedFats = totalSaturatedFats;
        TotalFiber = totalFiber;
        TotalSodium = totalSodium;
        TotalSalt = totalSalt;
        TotalCholesterol = totalCholesterol;
    }

    public static FoodEntry Create(Food.Food food, decimal quantityInGrams)
    {
        if (quantityInGrams <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantityInGrams));

        var factor = quantityInGrams / 100m;

        return new FoodEntry(
            food.Id,
            food.Name,
            food.Brand != null ? new Name(food.Brand) : null,
            quantityInGrams,
            Math.Round(food.NutritionPer100G.Calories * factor, 2),
            Math.Round(food.NutritionPer100G.Protein * factor, 2),
            Math.Round(food.NutritionPer100G.Carbohydrates * factor, 2),
            food.NutritionPer100G.Sugar.HasValue ? Math.Round(food.NutritionPer100G.Sugar.Value * factor, 2) : null,
            Math.Round(food.NutritionPer100G.Fat * factor, 2),
            food.NutritionPer100G.SaturatedFat.HasValue ? Math.Round(food.NutritionPer100G.SaturatedFat.Value * factor, 2) : null,
            food.NutritionPer100G.Fiber.HasValue ? Math.Round(food.NutritionPer100G.Fiber.Value * factor, 2) : null,
            food.NutritionPer100G.Sodium.HasValue ? Math.Round(food.NutritionPer100G.Sodium.Value * factor, 2) : null,
            food.NutritionPer100G.Salt.HasValue ? Math.Round(food.NutritionPer100G.Salt.Value * factor, 2) : null,
            food.NutritionPer100G.Cholesterol.HasValue ? Math.Round(food.NutritionPer100G.Cholesterol.Value * factor, 2) : null
        );
    }
}