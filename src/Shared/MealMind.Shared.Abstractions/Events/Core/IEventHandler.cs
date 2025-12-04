using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Abstractions.Events.Core;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent;