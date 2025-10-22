namespace MealMind.Modules.AiChat.Application.Dtos;

public sealed record StructuredResponse
{
    public string Title { get; init; } = string.Empty;
    public List<string> Paragraphs { get; init; } = [];
    public List<string> KeyPoints { get; init; } = [];
    public List<string> Sources { get; init; } = [];
}