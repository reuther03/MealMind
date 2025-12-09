using MealMind.Shared.Abstractions.Events.Core;
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
        var handlerInterface = typeof(IEventHandler<>)
            .MakeGenericType(notification.GetType());

        var handlers = _serviceProvider.GetServices(handlerInterface);

        foreach (var handlerObj in handlers)
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