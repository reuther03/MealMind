using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Tests.Unit;

public class NutritionTargetTest
{
    [Test]
    public async Task CreateNutritionTargetInGrams_ValidInput_ShouldCreateInstance()
    {
        // Arrange
        var userId = UserId.New();
        const decimal calories = 2000;
        const decimal protein = 143m;
        const decimal carbs = 200m;
        const decimal waterIntake = 2.5m;
        const decimal fats = 71m;

        // Act
        var nutritionTarget = NutritionTarget.CreateFromGrams(
            calories,
            protein,
            carbs,
            fats,
            waterIntake,
            userId
        );

        // Assert
        await Assert.That(nutritionTarget).IsNotNull();
        await Assert.That(nutritionTarget.ActualCalories)
            .IsGreaterThanOrEqualTo(1988)
            .And
            .IsLessThanOrEqualTo(2012);
        await Assert.That(nutritionTarget.IsActive).IsTrue();
        await Assert.That(nutritionTarget.CarbohydratesPercentage + nutritionTarget.ProteinPercentage + nutritionTarget.FatsPercentage).IsEqualTo(100);
    }

    [Test]
    public async Task CreateNutritionTargetInGrams_InvalidCalories_ShouldThrowException()
    {
        // Arrange
        var userId = UserId.New();
        const decimal calories = 2000;
        const decimal protein = 100m;
        const decimal carbs = 100m;
        const decimal waterIntake = 2.5m;
        const decimal fats = 50m;

        // Act & Assert
        await Assert.That(() => NutritionTarget.CreateFromGrams(
            calories,
            protein,
            carbs,
            fats,
            waterIntake,
            userId
        )).Throws<DomainException>().WithMessage($"Macro calories ({protein * 4 + carbs * 4 + fats * 9:F0}) don't match target calories ({calories:F0})");
    }

    [Test]
    public async Task CreateNutritionTargetInPercentages_ValidInput_ShouldCreateInstance()
    {
        // Arrange
        var userId = UserId.New();
        const decimal calories = 2000;
        const decimal proteinPercentage = 30m;
        const decimal carbsPercentage = 40m;
        const decimal waterIntake = 2.5m;
        const decimal fatsPercentage = 30m;

        // Act
        var nutritionTarget = NutritionTarget.CreateFromPercentages(
            calories,
            proteinPercentage,
            carbsPercentage,
            fatsPercentage,
            waterIntake,
            userId
        );

        // Assert
        await Assert.That(nutritionTarget).IsNotNull();
        await Assert.That(nutritionTarget.ActualCalories)
            .IsGreaterThanOrEqualTo(1988)
            .And
            .IsLessThanOrEqualTo(2012);
        await Assert.That(nutritionTarget.IsActive).IsTrue();
        await Assert.That(nutritionTarget.CarbohydratesPercentage + nutritionTarget.ProteinPercentage + nutritionTarget.FatsPercentage).IsEqualTo(100);
    }

    [Test]
    public async Task CreateNutritionTargetFromPercentages_RoundingEdgeCase_ShouldCreateInstance()
    {
        // Test percentages that might cause rounding issues
        var nutritionTarget = NutritionTarget.CreateFromPercentages(
            2000, 33, 33, 34, 2.5m, UserId.New());

        await Assert.That(nutritionTarget.ProteinPercentage +
            nutritionTarget.CarbohydratesPercentage +
            nutritionTarget.FatsPercentage).IsEqualTo(100);
    }

    [Test]
    public async Task CreateNutritionTargetInPercentages_InvalidTotalPercentage_ShouldThrowException()
    {
        // Arrange
        var userId = UserId.New();
        const decimal calories = 2000;
        const decimal proteinPercentage = 30m;
        const decimal carbsPercentage = 50m;
        const decimal waterIntake = 2.5m;
        const decimal fatsPercentage = 30m;

        // Act & Assert
        await Assert.That(() => NutritionTarget.CreateFromPercentages(
            calories,
            proteinPercentage,
            carbsPercentage,
            fatsPercentage,
            waterIntake,
            userId
        )).Throws<DomainException>().WithMessage($"Percentages must sum to 100% (currently {proteinPercentage + carbsPercentage + fatsPercentage}%)");
    }

    [Test]
    public async Task UserProfile_AddNutritionTarget_WithOverlappingDays_ShouldThrow()
    {
        // Arrange
        var userProfile = UserProfile.Create(UserId.New(), "test", "test@test.com", SubscriptionTier.Free);
        var target1 = NutritionTarget.CreateFromGrams(2000, 150, 200, 67, 3, UserId.New());
        target1.AddActiveDay([DayOfWeek.Monday, DayOfWeek.Tuesday]);

        var target2 = NutritionTarget.CreateFromGrams(2200, 160, 225, 72, 3, UserId.New());
        target2.AddActiveDay([DayOfWeek.Tuesday, DayOfWeek.Wednesday]);

        userProfile.AddNutritionTarget(target1);

        // Act & Assert
        await Assert.That(() => userProfile.AddNutritionTarget(target2))
            .Throws<InvalidOperationException>()
            .WithMessage("A nutrition target with overlapping active days already exists.");
    }
}