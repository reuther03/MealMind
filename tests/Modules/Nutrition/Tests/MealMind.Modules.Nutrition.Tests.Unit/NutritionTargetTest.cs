using MealMind.Modules.Nutrition.Domain.UserProfile;
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
        const decimal protein = 150m;
        const decimal carbs = 250m;
        const decimal waterIntake = 2.5m;
        const decimal fats = 70m;

        // Act
        var nutritionTarget = NutritionTarget.CreateFromGrams(
            calories,
            protein,
            carbs,
            fats,
            waterIntake,
            UserId.New()
        );

        var expectedProteinPercentage = Math.Round(protein * 4 / calories * 100, 1);
        var expectedCarbsPercentage = Math.Round(carbs * 4 / calories * 100, 1);
        var expectedFatsPercentage = Math.Round(fats * 9 / calories * 100, 1);

        // Assert
        await Assert.That(nutritionTarget).IsNotNull();
        await Assert.That(nutritionTarget.Calories).IsEqualTo(calories);
        await Assert.That(nutritionTarget.ProteinGrams).IsEqualTo(protein);
        await Assert.That(nutritionTarget.CarbohydratesGrams).IsEqualTo(carbs);
        await Assert.That(nutritionTarget.FatsGrams).IsEqualTo(fats);
        await Assert.That(nutritionTarget.WaterIntake).IsEqualTo(waterIntake);
        await Assert.That(nutritionTarget.UserProfileId).IsEqualTo(userId);
        await Assert.That(nutritionTarget.IsActive).IsTrue();


        await Assert.That(nutritionTarget.ProteinPercentage).IsEqualTo(expectedProteinPercentage);
        await Assert.That(nutritionTarget.CarbohydratesPercentage).IsEqualTo(expectedCarbsPercentage);
        await Assert.That(nutritionTarget.FatsPercentage).IsEqualTo(expectedFatsPercentage);
    }
}