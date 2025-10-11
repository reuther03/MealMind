using Pgvector;

namespace MealMind.Services.AiChat.Domain;

public class RagDocument
{
    public Guid Id { get; private set; }
    public string Content { get; private set; } = null!;
    public Vector Embedding { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private RagDocument() { }

    public static RagDocument Create(string content, Vector embedding)
    {
        return new RagDocument
        {
            Id = Guid.NewGuid(),
            Content = content,
            Embedding = embedding,
            CreatedAt = DateTime.UtcNow
        };
    }
}
