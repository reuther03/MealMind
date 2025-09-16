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
    public decimal TotalSugars { get; private set; }
    public decimal TotalFats { get; private set; }
    public decimal TotalSaturatedFats { get; private set; }
    public decimal TotalFiber { get; private set; }
    public decimal TotalSodium { get; private set; }

    private FoodEntry()
    {
    }

    private FoodEntry(Guid id, FoodId foodId, Name foodName, decimal quantityInGrams, decimal totalCalories, decimal totalProteins,
        decimal totalCarbohydrates, decimal totalSugars, decimal totalFats, decimal totalSaturatedFats, decimal totalFiber,
        decimal totalSodium, Name? foodBrand = null
    ) : base(id)
    {
        FoodId = foodId;
        FoodName = foodName;
        QuantityInGrams = quantityInGrams;
        TotalCalories = totalCalories;
        TotalProteins = totalProteins;
        TotalCarbohydrates = totalCarbohydrates;
        TotalSugars = totalSugars;
        TotalFats = totalFats;
        TotalSaturatedFats = totalSaturatedFats;
        TotalFiber = totalFiber;
        TotalSodium = totalSodium;
        FoodBrand = foodBrand;
    }

    public static FoodEntry Create(Guid id, FoodId foodId, Name foodName, decimal quantityInGrams, decimal totalCalories,
        decimal totalProteins, decimal totalCarbohydrates, decimal totalSugars, decimal totalFats, decimal totalSaturatedFats,
        decimal totalFiber, decimal totalSodium, Name? foodBrand = null
    ) => new(id, foodId, foodName, quantityInGrams, totalCalories, totalProteins, totalCarbohydrates, totalSugars,
        totalFats, totalSaturatedFats, totalFiber, totalSodium, foodBrand);
}