using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Infrastructure.Database;

internal class TrainingDbContext : DbContext, ITrainingDbContext
{
    public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
    public DbSet<Exercise> Exercises => Set<Exercise>();

    public TrainingDbContext(DbContextOptions<TrainingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.HasDefaultSchema("training");
    }
}