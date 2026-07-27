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
        foreach (var openHandler in new[] { typeof(IEventHandler<>), typeof(IDomainNotificationHandler<>) })
        {
            var handlerInterface = openHandler.MakeGenericType(notification.GetType());
            foreach (var handlerObj in _serviceProvider.GetServices(handlerInterface))
            {
                dynamic handler = handlerObj!;
                await handler.Handle((dynamic)notification, cancellationToken);
            }
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
