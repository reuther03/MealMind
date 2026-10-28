using MealMind.Modules.AiChat.Application.Abstractions.Database;

namespace MealMind.Evals.Retrieval;

public class RetrievalEvaluator
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentRepository _documentRepository;

    public RetrievalEvaluator(IEmbeddingService embeddingService, IDocumentRepository documentRepository)
    {
        _embeddingService = embeddingService;
        _documentRepository = documentRepository;
    }

    public async Task<IReadOnlyList<EmbeddedCase>> EmbedAsync(IEnumerable<RetrievalCase> cases, string? queryPrefix = null)
    {
        var embedded = new List<EmbeddedCase>();

        foreach (var retrievalCase in cases)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(queryPrefix + retrievalCase.Question);
            embedded.Add(new EmbeddedCase(retrievalCase, embedding.ToArray()));
        }

        return embedded;
    }

    public async Task<IReadOnlyList<CaseResult>> RunAsync(IReadOnlyList<EmbeddedCase> cases, int topK, double? maxDistance = null)
    {
        var results = new List<CaseResult>();

        foreach (var (retrievalCase, embedding) in cases)
        {
            var documents = await _documentRepository.GetRelevantDocumentsAsync(embedding, topK, maxDistance);
            var titles = documents.Select(d => d.Title).ToList();

            var rank = titles.IndexOf(retrievalCase.ExpectedDocument ?? string.Empty) + 1;

            results.Add(new CaseResult(retrievalCase, rank, titles));
        }

        return results;
    }
}
