using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class FoodStatistics : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public int TotalUsageCount { get; private set; }
    public int FavoriteCount { get; private set; }
    public double AverageRating { get; private set; }
    public int RatingCount { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public int SearchCount { get; private set; }

    public double PopularityScore
    {
        get => TotalUsageCount * 0.5 + FavoriteCount * 2 + SearchCount * 0.05;
        set { }
    }

    public double WeightedRating
    {
        get => RatingCount == 0 ? 0 : AverageRating * RatingCount / (RatingCount + 5.0);
        set { }
    }

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
        => new(Guid.NewGuid(), foodId, 0, 0, 0, 0, null, 0);
}