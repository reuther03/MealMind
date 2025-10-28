using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class WeightHistory : Entity<Guid>
{
    public UserId UserProfileId { get; private set; }
    public DateOnly Date { get; private set; }
    public decimal Weight { get; private set; }

    private WeightHistory()
    {
    }

    private WeightHistory(Guid id, UserId userProfileId, DateOnly date, decimal weight) : base(id)
    {
        UserProfileId = userProfileId;
        Date = date;
        Weight = weight;
    }

    public static WeightHistory Create(UserId userId, DateOnly date, decimal weight)
        => new(Guid.NewGuid(), userId, date, weight);
}