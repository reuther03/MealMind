using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class FoodDietaryTag : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public DietaryTag DietaryTag { get; private set; }

    private FoodDietaryTag()
    {
    }

    private FoodDietaryTag(Guid id, FoodId foodId, DietaryTag dietaryTag) : base(id)
    {
        FoodId = foodId;
        DietaryTag = dietaryTag;
    }


    public static FoodDietaryTag Create(FoodId foodId, DietaryTag dietaryTag)
        => new(Guid.NewGuid(), foodId, dietaryTag);
}