using System.Text.Json.Serialization;

namespace MealMind.Modules.AiChat.Application.Dtos;

public sealed record StructuredResponse
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("paragraphs")]
    public List<string> Paragraphs { get; init; } = [];

    [JsonPropertyName("keyPoints")]
    public List<string> KeyPoints { get; init; } = [];
}