using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;

namespace MealMind.Modules.Nutrition.Application.Dtos.DailyLogDtos;

public sealed class MealDto
{
    public MealType MealType { get; private set; }
    public string? Name { get; private set; }
    private IReadOnlyCollection<FoodEntryDto> Foods { get; init; } = [];
    public decimal TotalCalories => Foods.Sum(f => f.TotalCalories);
    public decimal TotalProteins => Foods.Sum(f => f.TotalProteins);
    public decimal TotalCarbohydrates => Foods.Sum(f => f.TotalCarbohydrates);
    public decimal? TotalSugars => Foods.Sum(f => f.TotalSugars);
    public decimal TotalFats => Foods.Sum(f => f.TotalFats);
    public decimal? TotalSaturatedFats => Foods.Sum(f => f.TotalSaturatedFats);
    public decimal? TotalFiber => Foods.Sum(f => f.TotalFiber);
    public decimal? TotalSodium => Foods.Sum(f => f.TotalSodium);

    public static MealDto AsDto(Meal meal)
        => new()
        {
            MealType = meal.MealType,
            Name = meal.Name?.Value,
            Foods = meal.Foods.Select(FoodEntryDto.AsDto).ToList()
        };
}