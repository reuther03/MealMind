using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Events.Identity;

public record SubscriptionTierAddedEvent(Guid Id) : Event(Guid.NewGuid());