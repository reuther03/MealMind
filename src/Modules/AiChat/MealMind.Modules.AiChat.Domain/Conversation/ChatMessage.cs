using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.AiChat.Domain.Conversation;

public class ChatMessage : Entity<Guid>
{
    public ConversationId ConversationId { get; private set; }
    public ChatRole Role { get; private set; }
    public string Content { get; private set; }
    public Guid ReplyToMessageId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ChatMessage()
    {
    }

    private ChatMessage(Guid id, Guid conversationId, ChatRole role, string content, Guid replyToMessageId, DateTime createdAt) : base(id)
    {
        ConversationId = conversationId;
        Role = role;
        Content = content;
        ReplyToMessageId = replyToMessageId;
        CreatedAt = createdAt;
    }

    public static ChatMessage Create(Guid conversationId, ChatRole role, string content, Guid replyToMessageId)
        => new(Guid.NewGuid(), conversationId, role, content, replyToMessageId, DateTime.UtcNow);
}