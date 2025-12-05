using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.Kernel.Payloads;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Shared.Events.Identity;

public record IdentityUserCreatedEvent(
    UserId Id,
    Name Username,
    Email Email,
    PersonalDataPayload PersonalData,
    List<NutritionTargetPayload> NutritionTargets
) : Event(Guid.NewGuid());