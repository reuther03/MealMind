namespace MealMind.Shared.Contracts.Dto.AiChat;

public class AiChatMessageDto
{
    public string Role { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public Guid ReplyToMessageId { get; private set; }
    public DateTime CreatedAt { get; private set; }
}