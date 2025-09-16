using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Abstractions.Events.Integration;

public record IdentityUserCreatedEvent(
    UserId Id,
    Name Username,
    Email Email,
    Gender Gender,
    DateOnly DateOfBirth,
    decimal Weight,
    decimal Height,
    decimal WeightTarget,
    ActivityLevel ActivityLevel) : INotification;