using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public class DailyLog : AggregateRoot<DailyLogId>
{
    private readonly List<Meal> _meals = [];
    public DateOnly CurrentDate { get; private set; }
    public decimal? CurrentWeight { get; private set; }
    public IReadOnlyList<Meal> Meals => _meals.AsReadOnly();
    public decimal CaloriesGoal { get; private set; }
    public decimal TotalCalories => _meals.Sum(m => m.TotalCalories);
    public decimal TotalProteins => _meals.Sum(m => m.TotalProteins);
    public decimal TotalCarbohydrates => _meals.Sum(m => m.TotalCarbohydrates);
    public decimal? TotalSugars => _meals.Sum(m => m.TotalSugars);
    public decimal TotalFats => _meals.Sum(m => m.TotalFats);
    public decimal? TotalSaturatedFats => _meals.Sum(m => m.TotalSaturatedFats);
    public decimal? TotalFiber => _meals.Sum(m => m.TotalFiber);
    public decimal? TotalSodium => _meals.Sum(m => m.TotalSodium);
    public UserId UserId { get; private set; }


    private DailyLog()
    {
    }

    private DailyLog(DailyLogId id, DateOnly currentDate, decimal? currentWeight, decimal caloriesGoal, UserId userId) : base(id)
    {
        CurrentDate = currentDate;
        CurrentWeight = currentWeight ?? 0;
        CaloriesGoal = caloriesGoal;
        UserId = userId;
    }

    public static DailyLog Create(DateOnly currentDate, decimal? currentWeight, decimal caloriesGoal, UserId userId) =>
        new(DailyLogId.New(), currentDate, currentWeight, caloriesGoal, userId);

    public void AddMeal(Meal meal)
    {
        if (_meals.Any(m => m.MealType == meal.MealType))
            throw new DomainException($"Meal of type {meal.MealType} already exists for this day.");

        _meals.Add(meal);
    }
}