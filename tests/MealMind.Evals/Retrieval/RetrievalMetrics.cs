namespace MealMind.Evals.Retrieval;

public static class RetrievalMetrics
{
    public static RetrievalReport Compute(IReadOnlyList<CaseResult> results, int topK)
    {
        var ranks = results.Where(r => r.Case.IsInDomain).Select(r => r.Rank).ToList();
        var outOfDomain = results.Where(r => !r.Case.IsInDomain).ToList();

        return new RetrievalReport(
            RecallAt1: RecallAt(ranks, 1),
            RecallAt3: RecallAt(ranks, 3),
            RecallAt6: RecallAt(ranks, 6),
            RecallAt10: topK >= 10 ? RecallAt(ranks, 10) : null,
            Mrr: ranks.Count == 0 ? 0 : ranks.Average(r => r == 0 ? 0 : 1.0 / r),
            OodRejectionRate: outOfDomain.Count == 0 ? null : (double)outOfDomain.Count(r => r.ReturnedTitles.Count == 0) / outOfDomain.Count);
    }

    private static double RecallAt(IReadOnlyList<int> ranks, int k)
        => ranks.Count == 0 ? 0 : (double)ranks.Count(r => r >= 1 && r <= k) / ranks.Count;
}
