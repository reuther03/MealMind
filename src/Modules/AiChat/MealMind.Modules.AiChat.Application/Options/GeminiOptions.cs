namespace MealMind.Modules.AiChat.Application.Options;

public class GeminiOptions
{
    public const string SectionName = "llm:gemini";

    public string ApiKey { get; init; } = null!;
    public string Model { get; init; } = null!;
}