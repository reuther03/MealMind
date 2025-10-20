using Pgvector;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IEmbeddingService
{
    Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    // Task<Vector> GenerateEmbeddingsAsync(IEnumerable<string> chunks, CancellationToken cancellationToken = default);
}