using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class NutritionTarget : Entity<Guid>
{
    private readonly List<NutritionTargetActiveDays> _activeDays = [];

    public decimal Calories { get; private set; }
    public decimal ProteinGrams { get; private set; }
    public decimal CarbohydratesGrams { get; private set; }
    public decimal FatsGrams { get; private set; }
    public decimal ProteinPercentage => CalculatePercentage(ProteinGrams * 4);
    public decimal CarbohydratesPercentage => CalculatePercentage(CarbohydratesGrams * 4);
    public decimal FatsPercentage => CalculatePercentage(FatsGrams * 9);
    public decimal WaterIntake { get; private set; }
    public IReadOnlyList<NutritionTargetActiveDays> ActiveDays => _activeDays.AsReadOnly();
    public bool IsActive { get; private set; }
    public DateOnly? DeactivatedAt { get; private set; }
    public UserId UserProfileId { get; private set; }

    private NutritionTarget()
    {
    }

    private NutritionTarget(
        Guid id,
        decimal calories,
        decimal proteinGrams,
        decimal carbohydratesGrams,
        decimal fatsGrams,
        decimal waterIntake,
        UserId userProfileId
    ) : base(id)
    {
        Calories = calories;
        ProteinGrams = proteinGrams;
        CarbohydratesGrams = carbohydratesGrams;
        FatsGrams = fatsGrams;
        WaterIntake = waterIntake;
        IsActive = true;
        UserProfileId = userProfileId;
    }

    public static NutritionTarget CreateFromGrams(
        decimal calories,
        decimal proteinGrams,
        decimal carbohydrateGrams,
        decimal fatGrams,
        decimal waterIntake,
        UserId userProfileId
    )
    {
        // Validate that macro calories match total calories within a tolerance
        // Protein and Carbs: 4 calories per gram, Fats: 9 calories
        var calculatedCalories = proteinGrams * 4 + carbohydrateGrams * 4 + fatGrams * 9;
        var tolerance = calories * 0.1m; //
        if (Math.Abs(calculatedCalories - calories) > tolerance)
            throw new DomainException($"Macro calories ({calculatedCalories:F0}) don't match target calories ({calories:F0})");

        return new NutritionTarget(
            Guid.NewGuid(),
            calories,
            proteinGrams,
            carbohydrateGrams,
            fatGrams,
            waterIntake,
            userProfileId
        );
    }

    public static NutritionTarget CreateFromPercentages(
        decimal calories,
        decimal proteinPercentage,
        decimal carbohydratesPercentage,
        decimal fatsPercentage,
        decimal waterIntake,
        UserId userProfileId
    )
    {
        // Validate percentages sum to 100
        var totalPercent = proteinPercentage + carbohydratesPercentage + fatsPercentage;
        if (Math.Abs(totalPercent - 100) > 0.04m) // Allowing a small tolerance for rounding
            throw new DomainException($"Percentages must sum to 100% (currently {totalPercent}%)");

        // Calculate grams from percentages
        // Protein and Carbs: 4 calories per gram, Fats: 9 calories
        // Example: For 2000 calories and 30% protein -> (30/100)*2000/4 = 150 grams of protein
        // Rounding to 1 decimal place for practicality
        var proteinGrams = Math.Round(proteinPercentage * calories / 400, 1);
        var carbohydratesGrams = Math.Round(carbohydratesPercentage * calories / 400, 1);
        var fatsGrams = Math.Round(fatsPercentage * calories / 900, 1);

        return new NutritionTarget(
            Guid.NewGuid(),
            calories,
            proteinGrams,
            carbohydratesGrams,
            fatsGrams,
            waterIntake,
            userProfileId
        );
    }

    public void AddActiveDay(List<DayOfWeek> dayOfWeek)
    {
        if (_activeDays.Any(ad => dayOfWeek.Contains(ad.DayOfWeek)))
            throw new InvalidOperationException($"Active day for {dayOfWeek} already exists.");

        foreach (var activeDay in dayOfWeek)
        {
            _activeDays.Add(NutritionTargetActiveDays.Create(Id, activeDay));
        }
    }

    private decimal CalculatePercentage(decimal calories)
        => Calories > 0 ? Math.Round(calories / Calories * 100, 1) : 0;
}