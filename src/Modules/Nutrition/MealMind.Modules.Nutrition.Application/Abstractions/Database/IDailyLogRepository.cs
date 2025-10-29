using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.Database;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface IDailyLogRepository : IRepository<DailyLog>
{
    Task<DailyLog?> GetByDateAsync(DateOnly date, UserId userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithDateAsync(DateOnly date, UserId userId, CancellationToken cancellationToken = default);
}