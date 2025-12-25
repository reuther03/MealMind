using System.Text.Json.Serialization;

namespace MealMind.Shared.Contracts.Dto.AiChat;

public sealed record StructuredResponse
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("paragraphs")]
    public List<string> Paragraphs { get; init; } = [];

    [JsonPropertyName("keyPoints")]
    public List<string> KeyPoints { get; init; } = [];
}