namespace MealMind.Modules.AiChat.Application.Dtos;

public class AnalyzedImageStructuredResponse
{
    public List<DetectedFoodResponse> DetectedFoods { get; init; } = [];
    public double TotalMinEstimatedCalories =>  DetectedFoods.Sum(f => f.MinEstimatedCalories);
    public double TotalMaxEstimatedCalories =>  DetectedFoods.Sum(f => f.MaxEstimatedCalories);
    public double TotalMinEstimatedProteins => DetectedFoods.Sum(f => f.MinEstimatedProteins);
    public double TotalMaxEstimatedProteins => DetectedFoods.Sum(f => f.MaxEstimatedProteins);
    public double TotalMinEstimatedFats => DetectedFoods.Sum(f => f.MinEstimatedFats);
    public double TotalMaxEstimatedFats => DetectedFoods.Sum(f => f.MaxEstimatedFats);
    public double TotalMinEstimatedCarbohydrates => DetectedFoods.Sum(f => f.MinEstimatedCarbohydrates);
    public double TotalMaxEstimatedCarbohydrates => DetectedFoods.Sum(f => f.MaxEstimatedCarbohydrates);
    public double TotalConfidenceScore => DetectedFoods.Average(f => f.ConfidenceScore);
    public string UserDescription { get; init; } = string.Empty;
}