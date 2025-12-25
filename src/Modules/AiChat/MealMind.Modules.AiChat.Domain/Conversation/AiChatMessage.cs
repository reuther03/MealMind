using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.AiChat.Domain.Conversation;

public class AiChatMessage : Entity<Guid>
{
    public ConversationId ConversationId { get; private set; }
    public AiChatRole Role { get; private set; }
    public string Content { get; private set; }
    public Guid ReplyToMessageId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AiChatMessage()
    {
    }

    private AiChatMessage(Guid id, Guid conversationId, AiChatRole role, string content, Guid replyToMessageId, DateTime createdAt) : base(id)
    {
        ConversationId = conversationId;
        Role = role;
        Content = content;
        ReplyToMessageId = replyToMessageId;
        CreatedAt = createdAt;
    }

    public static AiChatMessage Create(Guid conversationId, AiChatRole role, string content, Guid replyToMessageId)
        => new(Guid.NewGuid(), conversationId, role, content, replyToMessageId, DateTime.UtcNow);
}