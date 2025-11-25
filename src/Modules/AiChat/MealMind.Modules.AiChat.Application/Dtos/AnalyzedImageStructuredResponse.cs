namespace MealMind.Modules.AiChat.Application.Dtos;

public record AnalyzedImageStructuredResponse
{
    public List<DetectedFoodResponse> DetectedFoods { get; init; } = [];
    public string FoodName { get; init; } = string.Empty;
    public string UserDescription { get; init; } = string.Empty;
    public decimal TotalMinEstimatedCalories => DetectedFoods.Sum(f => f.MinEstimatedCalories);
    public decimal TotalMaxEstimatedCalories => DetectedFoods.Sum(f => f.MaxEstimatedCalories);
    public decimal TotalMinEstimatedProteins => DetectedFoods.Sum(f => f.MinEstimatedProteins);
    public decimal TotalMaxEstimatedProteins => DetectedFoods.Sum(f => f.MaxEstimatedProteins);
    public decimal TotalMinEstimatedFats => DetectedFoods.Sum(f => f.MinEstimatedFats);
    public decimal TotalMaxEstimatedFats => DetectedFoods.Sum(f => f.MaxEstimatedFats);
    public decimal TotalMinEstimatedCarbohydrates => DetectedFoods.Sum(f => f.MinEstimatedCarbohydrates);
    public decimal TotalMaxEstimatedCarbohydrates => DetectedFoods.Sum(f => f.MaxEstimatedCarbohydrates);
    public decimal TotalConfidenceScore => DetectedFoods.Average(f => f.ConfidenceScore);
    public byte[] ImageBytes { get; init; } = [];
}