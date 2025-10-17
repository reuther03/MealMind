using MealMind.Modules.AiChat.Application.Abstractions.Database;
using Microsoft.Extensions.AI;
using Pgvector;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        var embedding = await _embeddingGenerator.GenerateAsync(text, cancellationToken: cancellationToken);

        return new Vector(embedding.Vector.ToArray());
    }
}