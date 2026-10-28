using System.Text.Json;

namespace MealMind.Evals.Retrieval;

public record RetrievalCase(
    string Question,
    string? ExpectedDocument,
    string? ExpectedFact,
    string Category,
    string Language)
{
    public bool IsInDomain => Category == "in-domain";

    public static IReadOnlyList<RetrievalCase> LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<List<RetrievalCase>>(json, options) ?? [];
    }
}
