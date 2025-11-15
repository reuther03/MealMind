using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Abstractions.Events.Integration;

public record SubscriptionTierUpdatedEvent(UserId UserId, SubscriptionTier SubscriptionTier) : INotification;