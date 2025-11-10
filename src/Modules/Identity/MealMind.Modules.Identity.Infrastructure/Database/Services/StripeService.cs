using MealMind.Modules.Identity.Application.Abstractions.Services;
using MealMind.Modules.Identity.Application.Options;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace MealMind.Modules.Identity.Infrastructure.Database.Services;

public class StripeService : IStripeService
{
    private readonly StripeOptions _stripeOptions;

    public StripeService(IOptions<StripeOptions> stripeOptions)
    {
        _stripeOptions = stripeOptions.Value;
        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(Guid userId, SubscriptionTier subscriptionTier)
    {
        var amount = subscriptionTier switch
        {
            SubscriptionTier.Standard => _stripeOptions.StandardMonthly,
            SubscriptionTier.Premium => _stripeOptions.PremiumMonthly,
            _ => throw new ArgumentOutOfRangeException(nameof(subscriptionTier), "Invalid subscription tier")
        };

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{subscriptionTier} Subscription",
                            Description = "Monthly subscription for MealMind"
                        },
                        UnitAmount = (long)(amount * 100)
                    },
                    Quantity = 1
                }
            ],
            Mode = "payment",
            SuccessUrl = "http://localhost:5000/payment-success",
            CancelUrl = "http://localhost:5000/payment-cancel",
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "subscriptionTier", subscriptionTier.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }
}