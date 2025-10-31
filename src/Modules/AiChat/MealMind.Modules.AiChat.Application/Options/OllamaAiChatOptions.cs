namespace MealMind.Modules.AiChat.Application.Options;

public class OllamaAiChatOptions
{
    public const string SectionName = "llm:ollama";

    public string ChatModel { get; init; } = null!;
    public string EmbedModel { get; init; } = null!;
    public string Uri { get; init; } = null!;
}