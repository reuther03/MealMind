using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class NutritionTarget : Entity<Guid>
{
    public int Calories { get; private set; }
    public int Protein { get; private set; }
    public int Carbohydrates { get; private set; }
    public int Fats { get; private set; }
    public int WaterIntake { get; private set; }
    public bool IsActive { get; private set; }
    public DateOnly? DeactivatedAt { get; private set; }
    public UserId UserProfileId { get; private set; }

    private NutritionTarget()
    {
    }

    private NutritionTarget(Guid id, int calories, int protein, int carbohydrates, int fats, int waterIntake, Guid userProfileId) : base(id)
    {
        Calories = calories;
        Protein = protein;
        Carbohydrates = carbohydrates;
        Fats = fats;
        WaterIntake = waterIntake;
        IsActive = true;
        UserProfileId = userProfileId;
    }

    public static NutritionTarget Create(Guid id, int calories, int protein, int carbohydrates, int fats, int waterIntake, Guid userProfileId)
        => new(id, calories, protein, carbohydrates, fats, waterIntake, userProfileId);

    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}