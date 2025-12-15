using MealMind.Shared.Abstractions.Events.Core;

namespace MealMind.Shared.Abstractions.Services;

public interface IPublisher
{
    Task Publish(object notification, CancellationToken cancellationToken = default);

    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}