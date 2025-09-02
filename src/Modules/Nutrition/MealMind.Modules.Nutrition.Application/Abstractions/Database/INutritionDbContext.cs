using MealMind.Modules.Nutrition.Domain.UserProfile;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface INutritionDbContext
{
    DbSet<UserProfile> UserProfiles { get; }
}