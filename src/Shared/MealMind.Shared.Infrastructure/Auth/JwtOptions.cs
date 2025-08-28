namespace MealMind.Shared.Infrastructure.Auth;

public class JwtOptions
{
    internal const string SectionName = "jwt";

    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public TimeSpan Expiry { get; init; }
}