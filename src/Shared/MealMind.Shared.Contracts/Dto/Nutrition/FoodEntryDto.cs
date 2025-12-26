namespace MealMind.Shared.Contracts.Dto.Nutrition;

public sealed class FoodEntryDto
{
    public Guid? FoodId { get; init; }
    public string FoodName { get; init; } = null!;
    public string? FoodBrand { get; init; }
    public decimal QuantityInGrams { get; init; }
    public decimal TotalCalories { get; init; }
    public decimal TotalProteins { get; init; }
    public decimal TotalCarbohydrates { get; init; }
    public decimal? TotalSugars { get; init; }
    public decimal TotalFats { get; init; }
    public decimal? TotalSaturatedFats { get; init; }
    public decimal? TotalFiber { get; init; }
    public decimal? TotalSodium { get; init; }
    public decimal? TotalSalt { get; init; }
    public decimal? TotalCholesterol { get; init; }
    public string Source { get; init; } = null!;
}