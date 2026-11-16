using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Pgvector;

namespace MealMind.Modules.AiChat.Infrastructure.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly IOptionsSnapshot<ConfigOptions> _configOptions;

    public EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IOptionsSnapshot<ConfigOptions> configOptions)
    {
        _embeddingGenerator = embeddingGenerator;
        _configOptions = configOptions;
    }

    public async Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        var embedding = await _embeddingGenerator.GenerateAsync(text, cancellationToken: cancellationToken);

        return new Vector(embedding.Vector.ToArray());
    }

    public Task<Vector> GenerateQueryEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        text = $"Instruct: {_configOptions.Value.QueryInstruction}\nQuery: {text}";

        return GenerateEmbeddingAsync(text, cancellationToken);
    }
}
