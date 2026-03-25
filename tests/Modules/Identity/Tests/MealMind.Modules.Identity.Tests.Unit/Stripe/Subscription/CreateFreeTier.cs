using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Subscription;

public class CreateFreeTier
{
    [Test]
    public async Task CreateFreeTier_ShouldSetFreeTier()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();

        await Assert.That(subscription.Tier).IsEqualTo(SubscriptionTier.Free);
    }

    [Test]
    public async Task CreateFreeTier_ShouldHaveNullStripeFields()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();

        await Assert.That(subscription.StripeCustomerId).IsNull();
        await Assert.That(subscription.StripeSubscriptionId).IsNull();
        await Assert.That(subscription.SubscriptionStatus).IsNull();
    }

    [Test]
    public async Task CreateFreeTier_ShouldHaveNullDates()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();

        await Assert.That(subscription.SubscriptionStartedAt).IsNull();
        await Assert.That(subscription.CurrentPeriodStart).IsNull();
        await Assert.That(subscription.CurrentPeriodEnd).IsNull();
        await Assert.That(subscription.CanceledAt).IsNull();
    }
}