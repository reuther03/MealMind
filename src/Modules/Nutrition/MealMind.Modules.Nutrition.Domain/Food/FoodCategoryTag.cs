using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class FoodCategoryTag : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public FoodCategory Category { get; private set; }

    private FoodCategoryTag()
    {
    }

    private FoodCategoryTag(Guid id, FoodId foodId, FoodCategory category) : base(id)
    {
        FoodId = foodId;
        Category = category;
    }

    public static FoodCategoryTag Create(FoodId foodId, FoodCategory category)
        => new(Guid.NewGuid(), foodId, category);
}