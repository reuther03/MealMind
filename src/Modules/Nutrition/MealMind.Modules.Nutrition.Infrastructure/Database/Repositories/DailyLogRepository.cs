using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Repositories;

internal class DailyLogRepository : Repository<DailyLog, NutritionDbContext>, IDailyLogRepository
{
    private readonly NutritionDbContext _context;

    public DailyLogRepository(NutritionDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<DailyLog?> GetByIdAsync(DailyLogId id, CancellationToken cancellationToken)
        => await _context.DailyLogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}