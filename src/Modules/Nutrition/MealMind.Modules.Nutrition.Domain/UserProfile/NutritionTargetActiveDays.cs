using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class NutritionTargetActiveDays : Entity<Guid>
{
    public Guid NutritionTargetId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }

    private NutritionTargetActiveDays()
    {
    }

    private NutritionTargetActiveDays(Guid id, Guid nutritionTargetId, DayOfWeek dayOfWeek) : base(id)
    {
        NutritionTargetId = nutritionTargetId;
        DayOfWeek = dayOfWeek;
    }


    public static NutritionTargetActiveDays Create(Guid id, Guid nutritionTargetId, DayOfWeek dayOfWeek)
        => new(id, nutritionTargetId, dayOfWeek);
}