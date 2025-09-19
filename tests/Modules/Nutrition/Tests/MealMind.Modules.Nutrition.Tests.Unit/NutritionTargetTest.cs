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
        await Assert.That(nutritionTarget.Calories).IsEqualTo(calories);
        await Assert.That(nutritionTarget.ProteinGrams).IsEqualTo(protein);
        await Assert.That(nutritionTarget.CarbohydratesGrams).IsEqualTo(carbs);
        await Assert.That(nutritionTarget.FatsGrams).IsEqualTo(fats);
        await Assert.That(nutritionTarget.WaterIntake).IsEqualTo(waterIntake);
        await Assert.That(nutritionTarget.UserProfileId).IsEqualTo(userId);
        await Assert.That(nutritionTarget.IsActive).IsTrue();
        await Assert.That(nutritionTarget.CarbohydratesPercentage + nutritionTarget.ProteinPercentage + nutritionTarget.FatsPercentage).IsEqualTo(100);
    }
}