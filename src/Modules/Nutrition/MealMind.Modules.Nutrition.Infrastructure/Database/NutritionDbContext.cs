using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Infrastructure.Database;

internal class NutritionDbContext : DbContext, INutritionDbContext
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<FoodStatistics> FoodStatistics => Set<FoodStatistics>();
    public DbSet<FoodReview> FoodReviews => Set<FoodReview>();

    public NutritionDbContext(DbContextOptions<NutritionDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("nutrition");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}