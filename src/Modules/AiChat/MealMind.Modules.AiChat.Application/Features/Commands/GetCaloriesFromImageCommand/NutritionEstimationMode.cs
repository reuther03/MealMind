namespace MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;

public record NutritionEstimationMode
{
    public EstimationMode EstimationModeCalories { get; init; }
    public EstimationMode EstimationModeProtein { get; init; }
    public EstimationMode EstimationModeCarbohydrates { get; init; }
    public EstimationMode EstimationModeFats { get; init; }
}