using Pgvector;

namespace MealMind.Modules.AiChat.Domain.Rag;

public class RagDocument : Document
{
    private RagDocument()
    {
    }

    private RagDocument(Guid id, string title, string content, Vector? embedding, int chunkIndex, Guid documentGroupId)
        : base(id, title, content, embedding, chunkIndex, documentGroupId)
    {
    }

    public static RagDocument Create(string title, string content, Vector? embedding, int chunkIndex, Guid documentGroupId)
        => new(Guid.NewGuid(), title, content, embedding, chunkIndex, documentGroupId);
}