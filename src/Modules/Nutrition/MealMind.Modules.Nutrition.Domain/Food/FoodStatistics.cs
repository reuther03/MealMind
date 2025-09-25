using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class FoodStatistics : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public int TotalUsageCount { get; private set; }
    public int FavoriteCount { get; private set; }

    public double AverageRating { get; private set; }
    public int RatingCount { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public int SearchCount { get; private set; }

    private FoodStatistics()
    {
    }

    private FoodStatistics(Guid id, FoodId foodId, int totalUsageCount, int favoriteCount, double averageRating, int ratingCount, DateTime? lastUsedAt,
        int searchCount) : base(id)
    {
        FoodId = foodId;
        TotalUsageCount = totalUsageCount;
        FavoriteCount = favoriteCount;
        AverageRating = averageRating;
        RatingCount = ratingCount;
        LastUsedAt = lastUsedAt;
        SearchCount = searchCount;
    }

    public static FoodStatistics Create(FoodId foodId)
    {
        return new FoodStatistics(Guid.NewGuid(), foodId, 0, 0, 0, 0, null, 0);
    }
}