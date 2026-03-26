using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Subscription;

public class EnsureTierTest
{
    [Test]
    public async Task EnsureTier_ShouldUpdatePeriodAndStatus()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();
        var currentPeriodStart = DateTime.UtcNow;
        var currentPeriodEnd = DateTime.UtcNow.AddDays(30);

        var updated = subscription.EnsureTier(SubscriptionTier.Free, currentPeriodStart, currentPeriodEnd, "active");

        await Assert.That(updated.CurrentPeriodStart).IsEqualTo(currentPeriodStart);
        await Assert.That(updated.CurrentPeriodEnd).IsEqualTo(currentPeriodEnd);
        await Assert.That(updated.SubscriptionStatus).IsEqualTo("active");
    }

    [Test]
    public async Task EnsureTier_ShouldThrowIfTierDoesNotMatch()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();

        var ex = Assert.Throws<ApplicationException>(() =>
            subscription.EnsureTier(SubscriptionTier.Standard, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "active"));

        await Assert.That(ex?.Message).IsEqualTo("Invalid tier Standard");
    }
}