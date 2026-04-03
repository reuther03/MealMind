using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.AiChat.Tests.Unit.User;

public class SubscriptionTest
{
    [Test]
    public async Task EnsureTier_ValidData_ShouldReturnSubscription()
    {
        var subscription = Subscription.CreateFreeTier();
        var paidSub = subscription.UpdateToPaidTier(
            SubscriptionTier.Standard,
            "customerId",
            "subscriptionId",
            DateTime.Now,
            DateTime.Now,
            DateTime.Now.AddMonths(1),
            "active"
        );

        var result = paidSub.EnsureTier(
            paidSub.Tier,
            paidSub.CurrentPeriodStart!.Value,
            paidSub.CurrentPeriodEnd!.Value,
            paidSub.SubscriptionStatus!
        );

        await Assert.That(result.Tier).IsEqualTo(paidSub.Tier);
        await Assert.That(result.CurrentPeriodStart).IsEqualTo(paidSub.CurrentPeriodStart);
        await Assert.That(result.CurrentPeriodEnd).IsEqualTo(paidSub.CurrentPeriodEnd);
        await Assert.That(result.SubscriptionStatus).IsEqualTo(paidSub.SubscriptionStatus);
    }

    [Test]
    public async Task EnsureTier_InvalidTier_ShouldThrow()
    {
        var subscription = Subscription.CreateFreeTier();
        var tier = SubscriptionTier.Standard;

        await Assert.That(() => subscription.EnsureTier(
            tier,
            subscription.CurrentPeriodStart ?? DateTime.Now,
            subscription.CurrentPeriodEnd ?? DateTime.Now.AddMonths(1),
            "active"
        )).Throws<DomainException>().WithMessage($"Invalid tier {tier}");
    }
}