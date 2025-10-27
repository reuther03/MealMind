namespace MealMind.Shared.Contracts.Dto.Identity;

public record IdentityDto
{
    public string Id { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string SubscriptionTier { get; init; } = null!;
}