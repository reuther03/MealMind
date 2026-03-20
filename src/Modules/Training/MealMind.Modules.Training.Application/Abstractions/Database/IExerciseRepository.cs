using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Training.Application.Abstractions.Database;

public interface IExerciseRepository : IRepository<Exercise>
{
    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}