namespace MealMind.Shared.Abstractions.Events.Core;

public abstract record Event(Guid EventId) : IEvent;