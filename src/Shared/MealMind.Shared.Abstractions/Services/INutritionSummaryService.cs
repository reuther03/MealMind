using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Shared.Abstractions.Services;

public interface INutritionSummaryService
{
    Task<string> BuildSummaryAsync(UserId userId, int weeks, CancellationToken ct);
}