using MealMind.Shared.Abstractions.Kernel.Events;

namespace MealMind.Shared.Abstractions.Kernel.Primitives;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void RaiseDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}