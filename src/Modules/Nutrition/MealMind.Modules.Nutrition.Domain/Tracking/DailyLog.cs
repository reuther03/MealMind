using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public class DailyLog : Entity<Guid>
{

    private readonly List<FoodEntry> _foodEntries = [];
    public DateTime Date { get; private set; }
    public decimal CurrentWeight { get; private set; }
}