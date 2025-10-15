using MealMind.Shared.Abstractions.Kernel.Primitives;
using Pgvector;

namespace MealMind.Modules.AiChat.Domain.Rag;

public abstract class Document : AggregateRoot<Guid>
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public Vector? Embedding { get; private set; }
    public int ChunkIndex { get; private set; }
    public Guid DocumentGroupId { get; private set; }
    public DateTime AttachedAt { get; private set; }

    protected Document()
    {
    }

    protected Document(Guid id, string title, string content, Vector? embedding, int chunkIndex, Guid documentGroupId)
        : base(id)
    {
        Title = title;
        Content = content;
        Embedding = embedding;
        ChunkIndex = chunkIndex;
        DocumentGroupId = documentGroupId;
        AttachedAt = DateTime.UtcNow;
    }
}