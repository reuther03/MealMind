using Pgvector;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IEmbeddingService
{
    Task<Vector> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}