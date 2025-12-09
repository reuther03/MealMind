using MealMind.Shared.Abstractions.Events.Core;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}