namespace MealMind.Modules.AiChat.Application.Dtos;

public class DetectedFoodResponse
{
    public string FoodName { get; init; } = string.Empty;
    public double QuantityInGrams { get; init; }
    public double ConfidenceScore { get; init; }
    public double MinEstimatedCalories { get; init; }
    public double MaxEstimatedCalories { get; init; }
    public double MinEstimatedProteins { get; init; }
    public double MaxEstimatedProteins { get; init; }
    public double MinEstimatedFats { get; init; }
    public double MaxEstimatedFats { get; init; }
    public double MinEstimatedCarbohydrates { get; init; }
    public double MaxEstimatedCarbohydrates { get; init; }
}