using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.Database;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Application.Abstractions.Database;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetWithIncludesByIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<NutritionTarget?> GetNutritionTargetByIdAsync(Guid nutritionTargetId, UserId userId, CancellationToken cancellationToken = default);
}