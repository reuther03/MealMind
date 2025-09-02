using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.Database;

namespace MealMind.Modules.Identity.Application.Abstractions.Database;

public interface IIdentityUserRepository : IRepository<IdentityUser>
{
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}