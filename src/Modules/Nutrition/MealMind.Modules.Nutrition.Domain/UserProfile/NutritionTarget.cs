using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class NutritionTarget : Entity<Guid>
{
    private readonly List<DayOfWeek> _ActiveDays = [];

    public decimal Calories { get; private set; }
    public decimal Protein { get; private set; }
    public decimal Carbohydrates { get; private set; }
    public decimal Fats { get; private set; }
    public decimal WaterIntake { get; private set; }
    public IReadOnlyList<DayOfWeek> ActiveDays => _ActiveDays.AsReadOnly();
    public bool IsActive { get; private set; }
    public DateOnly? DeactivatedAt { get; private set; }
    public UserId UserProfileId { get; private set; }

    private NutritionTarget()
    {
    }

    private NutritionTarget(Guid id, decimal calories, decimal protein, decimal carbohydrates, decimal fats, decimal waterIntake, Guid userProfileId) : base(id)
    {
        Calories = calories;
        Protein = protein;
        Carbohydrates = carbohydrates;
        Fats = fats;
        WaterIntake = waterIntake;
        IsActive = true;
        UserProfileId = userProfileId;
    }

    public static NutritionTarget Create(decimal calories, decimal protein, decimal carbohydrates, decimal fats, decimal waterIntake, Guid userProfileId)
        => new(Guid.NewGuid(), calories, protein, carbohydrates, fats, waterIntake, userProfileId);

    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}