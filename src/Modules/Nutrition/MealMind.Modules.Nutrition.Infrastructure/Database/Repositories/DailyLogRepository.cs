using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
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

    public async Task<DailyLog?> GetByDateAsync(DateOnly date, Guid userId, CancellationToken cancellationToken = default)
        => await _context.DailyLogs
            .Include(x => x.Meals)
            .ThenInclude(x => x.Foods)
            .FirstOrDefaultAsync(x => x.CurrentDate == date && x.UserId.Value == userId, cancellationToken);

    public async Task<bool> ExistsWithDateAsync(DateOnly date, Guid userId, CancellationToken cancellationToken = default)
        => await _context.DailyLogs
            .AnyAsync(x => x.CurrentDate == date && x.UserId == UserId.From(userId), cancellationToken);
}