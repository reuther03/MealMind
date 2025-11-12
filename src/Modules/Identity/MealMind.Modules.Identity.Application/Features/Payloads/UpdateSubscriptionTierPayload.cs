using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Application.Features.Payloads;

public record UpdateSubscriptionTierPayload(
    Guid UserId,
    SubscriptionTier SubscriptionTier,
    string StripeCustomerId,
    string StripeSubscriptionId,
    DateTime SubscriptionStartedAt,
    DateTime CurrentPeriodStart,
    DateTime CurrentPeriodEnd,
    string SubscriptionStatus
);