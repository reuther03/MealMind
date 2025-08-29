using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Domain.IdentityUser;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Identity.Infrastructure.Database;

internal class IdentityDbContext : DbContext, IIdentityDbContext
{
    public DbSet<IdentityUser> IdentityUsers => Set<IdentityUser>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.Entity<IdentityUser>().HasQueryFilter(x => !x.IsDeleted);
    }
}