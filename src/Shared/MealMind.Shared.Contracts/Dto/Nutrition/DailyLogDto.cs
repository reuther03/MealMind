namespace MealMind.Shared.Contracts.Dto.Nutrition;

public sealed class DailyLogDto
{
    public DateOnly CurrentDate { get; init; }
    public decimal? CurrentWeight { get; init; }
    public IReadOnlyList<MealDto> Meals { get; init; } = [];
    public decimal CaloriesGoal { get; init; }
    public decimal TotalCalories => Meals.Sum(m => m.TotalCalories);
    public decimal TotalProteins => Meals.Sum(m => m.TotalProteins);
    public decimal TotalCarbohydrates => Meals.Sum(m => m.TotalCarbohydrates);
    public decimal? TotalSugars => Meals.Sum(m => m.TotalSugars);
    public decimal TotalFats => Meals.Sum(m => m.TotalFats);
    public decimal? TotalSaturatedFats => Meals.Sum(m => m.TotalSaturatedFats);
    public decimal? TotalFiber => Meals.Sum(m => m.TotalFiber);
    public decimal? TotalSodium => Meals.Sum(m => m.TotalSodium);
    public Guid UserId { get; init; }
}