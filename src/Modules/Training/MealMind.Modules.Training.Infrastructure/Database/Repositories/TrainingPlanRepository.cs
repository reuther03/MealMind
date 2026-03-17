using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Training.Infrastructure.Database.Repositories;

internal class TrainingPlanRepository : Repository<TrainingPlan, TrainingDbContext>, ITrainingPlanRepository
{
    private readonly TrainingDbContext _context;

    public TrainingPlanRepository(TrainingDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }
}