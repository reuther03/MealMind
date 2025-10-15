using MealMind.Modules.AiChat.Domain.Conversation;
using Pgvector;

namespace MealMind.Modules.AiChat.Domain.Rag;

public class ConversationDocument : Document
{
    public ConversationId ConversationId { get; private set; }

    private ConversationDocument()
    {
    }

    private ConversationDocument(Guid id, ConversationId conversationId, string title, string content, Vector? embedding, int chunkIndex, Guid documentGroupId)
        : base(id, title, content, embedding, chunkIndex, documentGroupId)
    {
        ConversationId = conversationId;
    }

    public static ConversationDocument Create(ConversationId conversationId, string title, string content, Vector? embedding, int chunkIndex,
        Guid documentGroupId)
        => new(Guid.NewGuid(), conversationId, title, content, embedding, chunkIndex, documentGroupId);
}