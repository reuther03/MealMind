using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface INutritionDbContext
{
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Food> Foods { get; }
    DbSet<DailyLog> DailyLogs { get; }
    DbSet<FoodStatistics> FoodStatistics { get; }
    DbSet<FoodReview> FoodReviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}