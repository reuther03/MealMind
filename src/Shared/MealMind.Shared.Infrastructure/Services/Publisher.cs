using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;
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
        var handlerInterface = typeof(INotificationHandler<>)
            .MakeGenericType(notification.GetType());

        var handlers = _serviceProvider.GetServices(handlerInterface);

        foreach (var handlerObj in handlers)
        {
            dynamic handler = handlerObj!;
            await handler.Handle((dynamic)notification, cancellationToken);
        }
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlerInterface = typeof(INotificationHandler<>)
            .MakeGenericType(typeof(TNotification));

        var handlers = _serviceProvider.GetServices(handlerInterface);

        foreach (var handlerObj in handlers)
        {
            dynamic handler = handlerObj!;
            await handler.Handle((dynamic)notification, cancellationToken);
        }
    }
}