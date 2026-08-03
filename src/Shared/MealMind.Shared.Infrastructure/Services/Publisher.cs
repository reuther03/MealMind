using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.Events;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Infrastructure.Services;

public sealed class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;
    public Publisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        var notificationType = notification.GetType();

        var openHandler = notification switch
        {
            IDomainEvent => typeof(IDomainNotificationHandler<>),
            IEvent => typeof(IEventHandler<>),
            _ => throw new InvalidOperationException($"Unknown notification type: {notificationType.Name}")
        };

        var handlerInterface = openHandler.MakeGenericType(notificationType);

        foreach (var handlerObj in _serviceProvider.GetServices(handlerInterface))
        {
            dynamic handler = handlerObj!;
            await handler.Handle((dynamic)notification, cancellationToken);
        }
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        var handlerInterface = typeof(IEventHandler<>)
            .MakeGenericType(typeof(TEvent));

        var handlers = _serviceProvider.GetServices(handlerInterface);

        foreach (var handlerObj in handlers)
        {
            dynamic handler = handlerObj!;
            await handler.Handle((dynamic)@event, cancellationToken);
        }
    }
}
