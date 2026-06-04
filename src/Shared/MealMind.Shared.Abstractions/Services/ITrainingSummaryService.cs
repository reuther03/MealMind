using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Shared.Abstractions.Services;

public interface ITrainingSummaryService
{
    Task<string> BuildSummaryAsync(UserId userId, CancellationToken ct);
}