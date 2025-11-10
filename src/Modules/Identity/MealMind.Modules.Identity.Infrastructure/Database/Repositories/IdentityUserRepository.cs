using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Identity.Infrastructure.Database.Repositories;

internal class IdentityUserRepository : Repository<IdentityUser, IdentityDbContext>, IIdentityUserRepository
{
    private readonly IdentityDbContext _context;

    public IdentityUserRepository(IdentityDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.IdentityUsers.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<IdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.IdentityUsers.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public Task<IdentityUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.IdentityUsers.FirstOrDefaultAsync(x => x.Id == UserId.From(id), cancellationToken);
}