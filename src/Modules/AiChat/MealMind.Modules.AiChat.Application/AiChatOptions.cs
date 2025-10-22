namespace MealMind.Modules.AiChat.Application;

public class AiChatOptions
{
    public const string SectionName = "llm";

    public string ChatModel { get; init; } = null!;
    public string EmbedModel { get; init; } = null!;
    public string Uri { get; init; } = null!;
    public float Temperature { get; init; }
    public int MaxOutputTokens { get; init; }
}