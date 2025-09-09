using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class FoodEntry : Entity<Guid>
{
    public MealType MealType { get; private set; }
    public FoodId FoodId { get; private set; }
    public decimal Quantity { get; private set; } // in grams
}