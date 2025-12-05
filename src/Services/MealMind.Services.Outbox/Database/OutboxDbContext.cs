using MealMind.Services.Outbox.OutboxEvents;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Services.Outbox.Database;

public class OutboxDbContext : DbContext
{
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

    public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("outbox");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}