using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Exception;
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

    [Test]
    public async Task Create_ValidDataWithUnevenQuantity_ShouldCreate()
    {
        var food = Food.Create("Test Food", new NutritionPer100G(330, 10, 30, 5, 8, 2, 0), FoodDataSource.Database);

        var foodEntry = FoodEntry.Create(food, 144);

        await Assert.That(foodEntry).IsNotNull();
        await Assert.That(foodEntry.FoodId!).IsEqualTo(food.Id);
        await Assert.That(foodEntry.QuantityInGrams).IsEqualTo(144);
        await Assert.That(foodEntry.TotalCalories).IsEqualTo(475.2m);
        await Assert.That(foodEntry.TotalProteins).IsEqualTo(14.4m);
        await Assert.That(foodEntry.TotalFats).IsEqualTo(43.2m);
        await Assert.That(foodEntry.TotalCarbohydrates).IsEqualTo(7.2m);
    }

    [Test]
    public async Task Create_InvalidQuantityInGrams_ShouldThrow()
    {
        var quantityInGrams = -1;

        var food = Food.Create("Test Food", new NutritionPer100G(330, 10, 30, 5, 8, 2, 0), FoodDataSource.Database);

        await Assert.That(() => FoodEntry.Create(food, quantityInGrams))
            .Throws<DomainException>()
            .WithMessage("Quantity must be greater than zero");
    }

    [Test]
    public async Task CreateFromImageAnalyze_ValidData_ShouldCreate()
    {
        var foodEntry = FoodEntry.CreateFromImageAnalyze("test food", 100, 330, 10, 30, 5);

        await Assert.That(foodEntry).IsNotNull();
        await Assert.That(foodEntry.FoodId).IsNotNull();
        await Assert.That(foodEntry.Source).IsEqualTo(FoodEntrySource.ImageAnalysis);
    }

    [Test]
    public async Task CreateFromImageAnalyze_InvalidQuantityInGrams_ShouldThrow()
    {
        await Assert.That(() => FoodEntry.CreateFromImageAnalyze("test food", -1, 330, 10, 30, 5))
            .Throws<DomainException>()
            .WithMessage("Quantity must be greater than zero");
    }
}