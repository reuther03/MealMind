namespace MealMind.Modules.Identity.Application.Options;

public class StripeOptions
{
    public const string SectionName = "stripe";

    public string PublishableKey { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public string WebhookSecret { get; init; } = null!;
    public double StandardMonthly { get; init; }
    public double PremiumMonthly { get; init; }
}