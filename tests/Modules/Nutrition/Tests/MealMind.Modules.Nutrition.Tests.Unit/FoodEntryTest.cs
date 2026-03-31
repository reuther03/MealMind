using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Nutrition.Tests.Unit;

public class FoodEntryTest
{
    [Test]
    public async Task Create_ValidData_ShouldCreate()
    {
        var food = Food.Create("Test Food", new NutritionPer100G(330, 10, 30, 5, 8, 2, 0), FoodDataSource.Database);

        var foodEntry = FoodEntry.Create(food, 100);

        await Assert.That(foodEntry).IsNotNull();
        await Assert.That(foodEntry.FoodId!).IsEqualTo(food.Id);
        await Assert.That(foodEntry.QuantityInGrams).IsEqualTo(100);
        await Assert.That(foodEntry.TotalCalories).IsEqualTo(330);
        await Assert.That(foodEntry.TotalProteins).IsEqualTo(10);
        await Assert.That(foodEntry.TotalFats).IsEqualTo(30);
        await Assert.That(foodEntry.TotalCarbohydrates).IsEqualTo(5);
    }
}