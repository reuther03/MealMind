using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Subscription;

public class CancelTest
{
    [Test]
    public async Task Cancel_ShouldUpdateCanceledAtAndStatus()
    {
        var subscription = Domain.IdentityUser.Subscription.CreateFreeTier();
        var canceledAt = DateTime.UtcNow;

        var updated = subscription.Cancel("test_customer_id", canceledAt, "canceled");

        await Assert.That(updated.CanceledAt).IsEqualTo(canceledAt);
        await Assert.That(updated.SubscriptionStatus).IsEqualTo("canceled");
        await Assert.That(updated.Tier).IsEqualTo(SubscriptionTier.Free);
    }
}