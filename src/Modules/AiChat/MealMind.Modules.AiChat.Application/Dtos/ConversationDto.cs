using MealMind.Modules.AiChat.Domain.Conversation;

namespace MealMind.Modules.AiChat.Application.Dtos;

public class ConversationDto
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUsedAt { get; private set; }

    public static ConversationDto AsDto(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id.Value,
            UserId = conversation.UserId.Value,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            LastUsedAt = conversation.LastUsedAt
        };
    }
}