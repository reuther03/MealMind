using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Domain.Rag;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Repositories;

internal class DocumentRepository : Repository<Document, AiChatDbContext>, IDocumentRepository
{
    private readonly AiChatDbContext _dbContext;

    public DocumentRepository(AiChatDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<IList<RagDocument>> GetRelevantDocumentsAsync(IEnumerable<float> embedding, int topK = 6, double? maxDistance = null, CancellationToken cancellationToken = default)
    {
        var queryVector = new Vector(embedding.ToArray());

        var query = _dbContext.RagDocuments.AsQueryable();

        if (maxDistance is not null)
            query = query.Where(x => x.Embedding!.CosineDistance(queryVector) < maxDistance);

        var relevantDocuments = await query
            .OrderBy(x => x.Embedding!.CosineDistance(queryVector))
            .Take(topK)
            .ToListAsync(cancellationToken);

        return relevantDocuments;
    }
}
