using MealMind.Shared.Abstractions.Events.Core;

namespace MealMind.Shared.Abstractions.Services;

public interface IOutboxService
{
    Task SaveAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}