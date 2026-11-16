namespace MealMind.Modules.AiChat.Application.Options;

public class ConfigOptions
{
    public const string SectionName = "llm:config";

    public double MaxDistance { get; init; }
    public string QueryInstruction { get; init; } = null!;
}
