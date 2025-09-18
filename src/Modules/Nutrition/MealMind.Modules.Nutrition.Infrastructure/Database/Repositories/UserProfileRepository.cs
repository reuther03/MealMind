using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Repositories;

internal class UserProfileRepository : Repository<UserProfile, NutritionDbContext>, IUserProfileRepository
{
    private readonly NutritionDbContext _dbContext;

    public UserProfileRepository(NutritionDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => await _dbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public Task<UserProfile?> GetWithIncludesByIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => _dbContext.UserProfiles
            .Include(x => x.NutritionTargets)
            .ThenInclude(x => x.ActiveDays)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
}