using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Shared.Events.Identity;

public record SubscriptionTierAddedEvent(UserId Id) : Event(Guid.NewGuid());