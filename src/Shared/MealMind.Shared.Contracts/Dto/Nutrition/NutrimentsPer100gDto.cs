namespace MealMind.Shared.Contracts.Dto.Nutrition;

public class NutrimentsPer100GDto
{
    public decimal Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbohydrates { get; init; }
    public decimal Fat { get; init; }
    public decimal? Fiber { get; init; }
    public decimal? Sugar { get; init; }
    public decimal? SaturatedFat { get; init; }
    public decimal? Sodium { get; init; }
    public decimal? Salt { get; init; }
    public decimal? Cholesterol { get; init; }
}