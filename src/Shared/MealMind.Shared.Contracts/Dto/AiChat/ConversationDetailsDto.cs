namespace MealMind.Shared.Contracts.Dto.AiChat;

public class ConversationDetailsDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public List<AiChatMessageDto> ChatMessages { get; init; } = [];
}