using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Training.Application.Abstractions.Database;

public interface ITrainingPlanRepository : IRepository<TrainingPlan>
{
    Task<TrainingPlan?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}