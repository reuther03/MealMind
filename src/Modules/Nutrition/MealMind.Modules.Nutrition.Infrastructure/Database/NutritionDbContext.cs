using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Infrastructure.Database;

internal class NutritionDbContext : DbContext, INutritionDbContext
{
    public NutritionDbContext(DbContextOptions<NutritionDbContext> options) : base(options)
    {
    }

    s

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("nutrition");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}