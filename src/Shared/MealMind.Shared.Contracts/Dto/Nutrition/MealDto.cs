namespace MealMind.Shared.Contracts.Dto.Nutrition;

public sealed class MealDto
{
    public Guid Id { get; init; }
    public int MealType { get; init; }
    public string? Name { get; init; }
    public IReadOnlyCollection<FoodEntryDto> Foods { get; init; } = [];
    public decimal TotalCalories => Foods.Sum(f => f.TotalCalories);
    public decimal TotalProteins => Foods.Sum(f => f.TotalProteins);
    public decimal TotalCarbohydrates => Foods.Sum(f => f.TotalCarbohydrates);
    public decimal? TotalSugars => Foods.Sum(f => f.TotalSugars);
    public decimal TotalFats => Foods.Sum(f => f.TotalFats);
    public decimal? TotalSaturatedFats => Foods.Sum(f => f.TotalSaturatedFats);
    public decimal? TotalFiber => Foods.Sum(f => f.TotalFiber);
    public decimal? TotalSodium => Foods.Sum(f => f.TotalSodium);
}