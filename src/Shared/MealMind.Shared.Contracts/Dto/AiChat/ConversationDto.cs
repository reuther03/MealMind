namespace MealMind.Shared.Contracts.Dto.AiChat;

public sealed class ConversationDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime LastUsedAt { get; init; }
}