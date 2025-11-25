namespace MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;

public class NutritionEstimationMode
{
    public EstimationMode Calories { get; init; }
    public EstimationMode Protein { get; init; }
    public EstimationMode Carbohydrates { get; init; }
    public EstimationMode Fats { get; init; }
}