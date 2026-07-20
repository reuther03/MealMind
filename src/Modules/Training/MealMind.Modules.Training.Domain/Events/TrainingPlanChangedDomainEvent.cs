using MealMind.Shared.Abstractions.Kernel.Events;

namespace MealMind.Modules.Training.Domain.Events;

public record TrainingPlanChangedDomainEvent(Guid UserId, Guid TrainingPlanId) : IDomainEvent;
