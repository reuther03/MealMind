using MealMind.Services.Outbox.Database;
using MealMind.Services.Outbox.OutboxEvents;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MealMind.Services.Outbox;

public class EventMessageProcessJob : BackgroundService
{
    private readonly ILogger<EventMessageProcessJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(10));
    private const int BatchSize = 20;

    public EventMessageProcessJob(ILogger<EventMessageProcessJob> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Worker running on: {Name}, at: {Time}", nameof(EventMessageProcessJob), DateTime.UtcNow);
                await ProcessEventMessageAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred executing {Name} at {Time}", nameof(EventMessageProcessJob), DateTime.UtcNow);
            }
        }
    }

    private async Task ProcessEventMessageAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var outboxDbContext = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();

        var outboxEvents = await outboxDbContext.OutboxEvents
            .Where(x => x.State == EventState.Pending || x.State == EventState.Failed)
            .OrderBy(x => x.CreatedOn)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (outboxEvents.Count == 0)
            return;

        foreach (var @event in outboxEvents)
        {
            try
            {
                _logger.LogInformation("Processing event: {Id} | {Name}", @event.Id, @event.GetType().Name);
                using var publisherScope = _serviceScopeFactory.CreateScope();
                var publisher = publisherScope.ServiceProvider.GetRequiredService<IPublisher>();
                await publisher.Publish(@event.Payload, cancellationToken);
                _logger.LogInformation("Event processed successfully: {Id} | {Name}", @event.Id, @event.GetType().Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process outbox message with ID: {Id} | {Name}.\n{Error}", @event.Id, @event.GetType().Name, e.Message);
                @event.SetFailed(e.Message);
                continue;
            }

            @event.SetProcessed();
        }

        await outboxDbContext.SaveChangesAsync(cancellationToken);
    }
}