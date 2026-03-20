using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Training.Infrastructure.Database.Repositories;

internal class ExerciseRepository : Repository<Exercise, TrainingDbContext>, IExerciseRepository
{
    private readonly TrainingDbContext _context;

    public ExerciseRepository(TrainingDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Exercises.FindAsync([id], cancellationToken);
}