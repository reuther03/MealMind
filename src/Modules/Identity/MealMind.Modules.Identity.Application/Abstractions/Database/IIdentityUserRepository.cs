using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Identity.Application.Abstractions.Database;

public interface IIdentityUserRepository : IRepository<IdentityUser>
{
}