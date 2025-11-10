using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Application.Abstractions.Services;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(Guid userId, SubscriptionTier subscriptionTier);
}