using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Subscription;

public class UpdateToPaidTierTest
{
    [Test]
    public async Task UpdateToPaidTier_ShouldSetAllFields()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();
        var startedAt = DateTime.UtcNow;
        var periodStart = DateTime.UtcNow;
        var periodEnd = DateTime.UtcNow.AddDays(30);

        var updated = subscription.UpdateToPaidTier(
            SubscriptionTier.Standard, "stripeId", "stripeSubId",
            startedAt, periodStart, periodEnd, "active");

        await Assert.That(updated.Tier).IsEqualTo(SubscriptionTier.Standard);
        await Assert.That(updated.StripeCustomerId).IsEqualTo("stripeId");
        await Assert.That(updated.StripeSubscriptionId).IsEqualTo("stripeSubId");
        await Assert.That(updated.SubscriptionStartedAt).IsEqualTo(startedAt);
        await Assert.That(updated.CurrentPeriodStart).IsEqualTo(periodStart);
        await Assert.That(updated.CurrentPeriodEnd).IsEqualTo(periodEnd);
        await Assert.That(updated.SubscriptionStatus).IsEqualTo("active");
    }
}