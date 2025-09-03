using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Repositories;

internal class UserProfileRepository : Repository<UserProfile, NutritionDbContext>, IUserProfileRepository
{
    public UserProfileRepository(NutritionDbContext dbContext) : base(dbContext)
    {
    }
}