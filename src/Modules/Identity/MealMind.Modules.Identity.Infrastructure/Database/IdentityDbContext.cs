using MealMind.Modules.Identity.Application.Abstractions.Database;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Identity.Infrastructure.Database;

internal class IdentityDbContext : DbContext, IIdentityDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}