using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public class DailyLog : Entity<Guid>
{
    public DateTime StartDate { get; private set; }
    public decimal CurrentWeight { get; private set; }
}