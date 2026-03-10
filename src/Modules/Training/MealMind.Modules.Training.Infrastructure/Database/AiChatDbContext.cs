using MealMind.Modules.Training.Application.Abstractions.Database;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Infrastructure.Database;

internal class TrainingDbContext : DbContext, ITrainingDbContext
{
    public TrainingDbContext(DbContextOptions<TrainingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.HasDefaultSchema("training");
    }
}