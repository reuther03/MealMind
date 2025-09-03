using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface IUserProfileRepository : IRepository<UserProfile>
{

}