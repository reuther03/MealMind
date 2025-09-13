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
}