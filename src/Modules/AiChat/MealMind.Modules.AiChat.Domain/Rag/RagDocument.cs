using MealMind.Shared.Abstractions.Kernel.Primitives;
using Pgvector;

namespace MealMind.Modules.AiChat.Domain.Rag;

public class RagDocument : Entity<Guid>
{
    public string Content { get; private set; }
    public Vector? Embedding { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RagDocument()
    {
    }

    private RagDocument(Guid id, string content, Vector? embedding) : base(id)
    {
        Content = content;
        Embedding = embedding;
        CreatedAt = DateTime.UtcNow;
    }

    public static RagDocument Create(string content, Vector? embedding)
        => new(Guid.NewGuid(), content, embedding);
}