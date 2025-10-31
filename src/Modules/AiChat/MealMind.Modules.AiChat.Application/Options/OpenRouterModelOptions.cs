namespace MealMind.Modules.AiChat.Application.Options;

public class OpenRouterModelOptions
{
    public const string SectionName = "llm:openRouter";

    public string ApiKey { get; init; } = null!;
    public string BaseUrl { get; init; } = null!;
    public string BaseModel { get; init; } = null!;
    public string VisionModel { get; init; } = null!;
}