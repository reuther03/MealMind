using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface IDailyLogRepository : IRepository<DailyLog>
{
    Task<DailyLog?> GetByIdAsync(DailyLogId id, CancellationToken cancellationToken);
}