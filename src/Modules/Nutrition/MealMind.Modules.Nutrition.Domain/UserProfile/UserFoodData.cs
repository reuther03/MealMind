using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class UserFoodData : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public UserId UserId { get; private set; }
    public int? Rating { get; private set; }
    public bool IsFavorite { get; private set; }
    public int TimesUsed { get; private set; }
    public DateTime? LastUsedAt { get; private set; }

    private UserFoodData()
    {
    }

    private UserFoodData(Guid id, FoodId foodId, UserId userId) : base(id)
    {
        FoodId = foodId;
        UserId = userId;
        TimesUsed = 0;
        IsFavorite = false;
    }

    public static UserFoodData Create(FoodId foodId, UserId userId)
        => new(Guid.NewGuid(), foodId, userId);

    public void UpdateRating(int rating)
    {
        if (rating is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        Rating = rating;
    }

    public void IncrementUsage()
    {
        TimesUsed++;
        LastUsedAt = DateTime.UtcNow;
    }
}