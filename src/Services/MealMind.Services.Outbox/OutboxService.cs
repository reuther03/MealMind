using MealMind.Services.Outbox.Database;
using MealMind.Services.Outbox.OutboxEvents;
using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Services.Outbox;

public class OutboxService : IOutboxService
{
    private readonly IServiceProvider _serviceProvider;

    public OutboxService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SaveAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxDbContext = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();

        var outboxEvent = OutboxEvent.Create(Guid.NewGuid(), @event.GetType(), @event);
        await outboxDbContext.OutboxEvents.AddAsync(outboxEvent, cancellationToken);

        var result = await outboxDbContext.SaveChangesAsync(cancellationToken) > 0;

        if (!result)
        {
            throw new InvalidOperationException($"Failed to insert '{@event.GetType().FullName}' event into outbox");
        }
    }
}