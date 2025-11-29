using MealMind.Modules.Nutrition.Domain.Tracking;

namespace MealMind.Modules.Nutrition.Application.Dtos.DailyLogDtos;

public sealed class DailyLogDto
{
    public DateOnly CurrentDate { get; private set; }
    public decimal? CurrentWeight { get; private set; }
    public IReadOnlyList<MealDto> Meals { get; init; } = [];
    public decimal CaloriesGoal { get; private set; }
    public decimal TotalCalories => Meals.Sum(m => m.TotalCalories);
    public decimal TotalProteins => Meals.Sum(m => m.TotalProteins);
    public decimal TotalCarbohydrates => Meals.Sum(m => m.TotalCarbohydrates);
    public decimal? TotalSugars => Meals.Sum(m => m.TotalSugars);
    public decimal TotalFats => Meals.Sum(m => m.TotalFats);
    public decimal? TotalSaturatedFats => Meals.Sum(m => m.TotalSaturatedFats);
    public decimal? TotalFiber => Meals.Sum(m => m.TotalFiber);
    public decimal? TotalSodium => Meals.Sum(m => m.TotalSodium);
    public Guid UserId { get; private set; }

    public static DailyLogDto AsDto(DailyLog dailyLog)
        => new()
        {
            CurrentDate = dailyLog.CurrentDate,
            CurrentWeight = dailyLog.CurrentWeight,
            Meals = dailyLog.Meals.Select(MealDto.AsDto).ToList(),
            CaloriesGoal = dailyLog.CaloriesGoal,
            UserId = dailyLog.UserId.Value
        };
}