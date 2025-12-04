using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Abstractions.Events.Core;

public interface IEvent : INotification
{
    public Guid EventId { get; }
}