using MealMind.Modules.Training.Domain.TrainingPlan;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Abstractions.Database;

public interface ITrainingDbContext
{
    DbSet<TrainingPlan> TrainingPlans { get; }
    DbSet<Exercise> Exercises { get; }
}