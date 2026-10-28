using MealMind.Evals.Retrieval;
using MealMind.Modules.AiChat.Application;
using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.AddApplication(configuration);
services.AddInfrastructure(configuration);

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var evaluator = new RetrievalEvaluator(
    scope.ServiceProvider.GetRequiredService<IEmbeddingService>(),
    scope.ServiceProvider.GetRequiredService<IDocumentRepository>());

var cases = RetrievalCase.LoadFromFile(Path.Combine(AppContext.BaseDirectory, "Data", "retrieval-set.json"));
var embedded = await evaluator.EmbedAsync(cases);

Console.WriteLine($"Cases: {cases.Count} ({cases.Count(c => c.IsInDomain)} in-domain, {cases.Count(c => !c.IsInDomain)} out-of-domain)");
Console.WriteLine();

// Baseline: no distance threshold
const int baselineTopK = 10;
var baseline = RetrievalMetrics.Compute(await evaluator.RunAsync(embedded, baselineTopK), baselineTopK);

Console.WriteLine("Baseline (no threshold)");
Console.WriteLine($"  recall@1: {baseline.RecallAt1:F2}  recall@3: {baseline.RecallAt3:F2}  recall@6: {baseline.RecallAt6:F2}  recall@10: {baseline.RecallAt10:F2}  MRR: {baseline.Mrr:F2}");
Console.WriteLine();

// Threshold sweep with the production topK
const int productionTopK = 6;
double[] thresholds = [0.70, 0.72, 0.74, 0.76, 0.78, 0.80, 0.82, 0.84, 0.86, 0.88, 0.90];

Console.WriteLine("Threshold sweep");
foreach (var threshold in thresholds)
{
    var report = RetrievalMetrics.Compute(await evaluator.RunAsync(embedded, productionTopK, threshold), productionTopK);

    Console.WriteLine($"  {threshold:F2}  recall@6: {report.RecallAt6:F2}  MRR: {report.Mrr:F2}  OOD rejected: {report.OodRejectionRate:F2}");
}

return 0;
