using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class FoodReview : Entity<Guid>
{
    public FoodId FoodId { get; private set; }
    public UserId UserId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private FoodReview()
    {
    }

    private FoodReview(Guid id, FoodId foodId, UserId userId, int rating, string? comment) : base(id)
    {
        FoodId = foodId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
    }

    public static FoodReview Create(Guid id, FoodId foodId, UserId userId, int rating, string? comment)
    {
        return rating is < 1 or > 5
            ? throw new DomainException("Rating must be between 1 and 5.", nameof(rating))
            : new FoodReview(id, foodId, userId, rating, comment);
    }
}