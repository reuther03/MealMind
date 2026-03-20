using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Infrastructure.Database.Repositories;

internal class TrainingPlanRepository : Repository<TrainingPlan, TrainingDbContext>, ITrainingPlanRepository
{
    private readonly TrainingDbContext _context;

    public TrainingPlanRepository(TrainingDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<TrainingPlan?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        => await _context.TrainingPlans
            .FirstOrDefaultAsync(x => x.Id == TrainingPlanId.From(id) &&
                x.UserId == UserId.From(userId), cancellationToken);
}