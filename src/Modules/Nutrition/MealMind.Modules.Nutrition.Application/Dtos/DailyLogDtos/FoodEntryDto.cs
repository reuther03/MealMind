using MealMind.Modules.Nutrition.Domain.Tracking;

namespace MealMind.Modules.Nutrition.Application.Dtos.DailyLogDtos;

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
    public FoodEntrySource Source { get; init; }

    public static FoodEntryDto AsDto(FoodEntry foodEntry)
        => new()
        {
            FoodId = foodEntry.FoodId?.Value,
            FoodName = foodEntry.FoodName.Value,
            FoodBrand = foodEntry.FoodBrand?.Value,
            QuantityInGrams = foodEntry.QuantityInGrams,
            TotalCalories = foodEntry.TotalCalories,
            TotalProteins = foodEntry.TotalProteins,
            TotalCarbohydrates = foodEntry.TotalCarbohydrates,
            TotalSugars = foodEntry.TotalSugars,
            TotalFats = foodEntry.TotalFats,
            TotalSaturatedFats = foodEntry.TotalSaturatedFats,
            TotalFiber = foodEntry.TotalFiber,
            TotalSodium = foodEntry.TotalSodium,
            TotalSalt = foodEntry.TotalSalt,
            TotalCholesterol = foodEntry.TotalCholesterol,
            Source = foodEntry.Source
        };
}