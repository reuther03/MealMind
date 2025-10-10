namespace MealMind.Services.AiChat;

public class AiChatOptions
{
    public const string SectionName = "llm";

    public string ChatModel { get; init; } = null!;
    public string EmbedModel { get; init; } = null!;
    public string Uri { get; init; } = null!;
}