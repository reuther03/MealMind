namespace MealMind.Shared.Infrastructure.Auth;

public class GoogleSettings
{
    internal const string SectionName = "google";

    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
}