namespace MealMind.Evals.Retrieval;

public record EmbeddedCase(RetrievalCase Case, float[] Embedding);

// Rank = 0 means the expected document was not returned; empty ReturnedTitles means the query was rejected.
public record CaseResult(RetrievalCase Case, int Rank, IReadOnlyList<string> ReturnedTitles);

// RecallAt10 is null when fewer than 10 documents were requested; OodRejectionRate is null when there are no out-of-domain cases.
public record RetrievalReport(
    double RecallAt1,
    double RecallAt3,
    double RecallAt6,
    double? RecallAt10,
    double Mrr,
    double? OodRejectionRate);
