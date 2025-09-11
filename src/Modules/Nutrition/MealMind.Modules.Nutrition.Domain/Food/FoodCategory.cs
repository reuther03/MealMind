using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class FoodCategory : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public Category Category { get; private set; }

    private FoodCategory()
    {
    }

    private FoodCategory(Guid id, FoodId foodId, Category category) : base(id)
    {
        FoodId = foodId;
        Category = category;
    }

    public static FoodCategory Create(FoodId foodId, Category category)
        => new(Guid.NewGuid(), foodId, category);
}