using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Shared.Abstractions.Services;

public interface INutritionSummaryService
{
    Task<string> BuildSummaryAsync(UserId userId, CancellationToken ct);
}