using MealMind.Modules.AiChat.Domain.Rag;
using MealMind.Shared.Abstractions.Kernel.Database;
using Document = MealMind.Modules.AiChat.Domain.Rag.Document;

namespace MealMind.Modules.AiChat.Application.Abstractions.Database;

public interface IDocumentRepository : IRepository<Document>
{
    Task<IList<RagDocument>> GetRelevantDocumentsAsync(IEnumerable<float> embedding, CancellationToken cancellationToken = default);
}