using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Infrastructure.Postgres;

namespace MealMind.Modules.Identity.Infrastructure.Database.Repositories;

internal class IdentityUserRepository : Repository<IdentityUser, IdentityDbContext>, IIdentityUserRepository
{
    public IdentityUserRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }
}