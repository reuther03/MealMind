using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using Pgvector;

namespace MealMind.Modules.AiChat.Domain.Rag;

public class ConversationDocument : Entity<Guid>
{
    public ConversationId ConversationId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime AttachedAt { get; private set; }
    public Vector? Embedding { get; private set; }

    private ConversationDocument()
    {
    }

    private ConversationDocument(Guid id, ConversationId conversationId, string title, string content, Vector? embedding) : base(id)
    {
        ConversationId = conversationId;
        Title = title;
        Content = content;
        Embedding = embedding;
        AttachedAt = DateTime.UtcNow;
    }

    public static ConversationDocument Create(ConversationId conversationId, string title, string content, Vector? embedding)
        => new(Guid.NewGuid(), conversationId, title, content, embedding);
}