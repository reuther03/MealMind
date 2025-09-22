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
    public decimal ActualCalories => ProteinGrams * 4 + CarbohydratesGrams * 4 + FatsGrams * 9;
    public decimal WaterIntake { get; private set; }
    public IReadOnlyList<NutritionTargetActiveDays> ActiveDays => _activeDays.AsReadOnly();
    public bool IsActive { get; private set; }
    public DateOnly? DeactivatedAt { get; private set; }
    public UserId UserProfileId { get; private set; }

    //Todo: add sodium, sugar, fiber, saturated fat, cholesterol targets?

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
        var calculatedCalories = proteinGrams * 4 + carbohydrateGrams * 4 + fatGrams * 9;
        const decimal calorieMargin = 12m;

        if (Math.Abs(calculatedCalories - calories) > calorieMargin)
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
        const decimal calorieMargin = 12m;

        var totalPercent = proteinPercentage + carbohydratesPercentage + fatsPercentage;
        if (totalPercent != 100)
            throw new DomainException($"Percentages must sum to 100% (currently {totalPercent}%)");

        var proteinGrams = Math.Round(proteinPercentage * calories / 400, 1);
        var carbohydratesGrams = Math.Round(carbohydratesPercentage * calories / 400, 1);
        var fatsGrams = Math.Round(fatsPercentage * calories / 900, 1);

        var calculatedCalories = proteinGrams * 4 + carbohydratesGrams * 4 + fatsGrams * 9;

        if (Math.Abs(calculatedCalories - calories) > calorieMargin)
            throw new DomainException($"Calculated calories ({calculatedCalories:F0}) don't match target calories ({calories:F0})");

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
        => ActualCalories > 0 ? Math.Round(calories / ActualCalories * 100, 0, MidpointRounding.ToEven) : 0;
}