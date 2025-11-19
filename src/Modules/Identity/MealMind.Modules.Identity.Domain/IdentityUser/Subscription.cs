using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Identity.Domain.IdentityUser;

public record Subscription : ValueObject
{
    public SubscriptionTier Tier { get; private init; }
    public string? StripeCustomerId { get; private init; }
    public string? StripeSubscriptionId { get; private init; }
    public DateTime? SubscriptionStartedAt { get; private init; }
    public DateTime? CurrentPeriodStart { get; private init; }
    public DateTime? CurrentPeriodEnd { get; private init; }
    public DateTime? CanceledAt { get; private init; }
    public string? SubscriptionStatus { get; private init; }

    private Subscription(SubscriptionTier tier, string? stripeCustomerId, string? stripeSubscriptionId,
        DateTime? subscriptionStartedAt, DateTime? currentPeriodStart, DateTime? currentPeriodEnd,
        DateTime? canceledAt, string? subscriptionStatus)
    {
        Tier = tier;
        StripeCustomerId = stripeCustomerId;
        StripeSubscriptionId = stripeSubscriptionId;
        SubscriptionStartedAt = subscriptionStartedAt;
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        CanceledAt = canceledAt;
        SubscriptionStatus = subscriptionStatus;
    }

    public static Subscription CreateFreeTier()
        => new(SubscriptionTier.Free, null, null, null, null, null, null, null);

    public Subscription UpdateToPaidTier(SubscriptionTier tier, string stripeCustomerId, string stripeSubscriptionId,
        DateTime? subscriptionStartedAt, DateTime currentPeriodStart, DateTime currentPeriodEnd,
        string subscriptionStatus)
        => this with
        {
            Tier = tier, StripeCustomerId = stripeCustomerId, StripeSubscriptionId = stripeSubscriptionId,
            SubscriptionStartedAt = subscriptionStartedAt, CurrentPeriodStart = currentPeriodStart, CurrentPeriodEnd = currentPeriodEnd,
            SubscriptionStatus = subscriptionStatus
        };

    public Subscription EnsureTier(SubscriptionTier tier, DateTime currentPeriodStart, DateTime currentPeriodEnd,
        string subscriptionStatus)
    {
        if (tier != Tier)
            throw new ApplicationException($"Invalid tier {tier}");

        var subscription = this with
        {
            CurrentPeriodStart = currentPeriodStart,
            CurrentPeriodEnd = currentPeriodEnd,
            SubscriptionStatus = subscriptionStatus
        };

        return subscription;
    }

    public Subscription Cancel(string stripeCustomerId, DateTime? canceledAt, string status)
        // ReSharper disable once WithExpressionModifiesAllMembers
        => this with
        {
            Tier = SubscriptionTier.Free,
            StripeCustomerId = stripeCustomerId,
            StripeSubscriptionId = null,
            SubscriptionStartedAt = null,
            CurrentPeriodStart = null,
            CurrentPeriodEnd = null,
            CanceledAt = canceledAt ?? DateTime.UtcNow,
            SubscriptionStatus = status
        };

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Tier;
        yield return StripeCustomerId ?? string.Empty;
        yield return StripeSubscriptionId ?? string.Empty;
        yield return SubscriptionStartedAt ?? default(DateTime);
        yield return CurrentPeriodStart ?? default(DateTime);
        yield return CurrentPeriodEnd ?? default(DateTime);
        yield return CanceledAt ?? default(DateTime);
        yield return SubscriptionStatus ?? string.Empty;
    }
}