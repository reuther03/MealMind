using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Abstractions.Services;

public interface IPublisher
{
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}