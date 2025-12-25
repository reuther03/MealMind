namespace MealMind.Shared.Contracts.Dto.AiChat;

public class AiChatMessageDto
{
    public Guid Id { get; init; }
    public string Role { get; init; } = null!;
    public string Content { get; init; } = null!;
    public Guid ReplyToMessageId { get; init; }
    public DateTime CreatedAt { get; init; }
}