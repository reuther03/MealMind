namespace MealMind.Modules.AiChat.Application.Dtos;

public class DetectedFoodResponse
{
    public string FoodName { get; init; } = string.Empty;
    public double QuantityInGrams { get; init; }
    public decimal ConfidenceScore { get; init; }
    public decimal MinEstimatedCalories { get; init; }
    public decimal MaxEstimatedCalories { get; init; }
    public decimal MinEstimatedProteins { get; init; }
    public decimal MaxEstimatedProteins { get; init; }
    public decimal MinEstimatedFats { get; init; }
    public decimal MaxEstimatedFats { get; init; }
    public decimal MinEstimatedCarbohydrates { get; init; }
    public decimal MaxEstimatedCarbohydrates { get; init; }
}