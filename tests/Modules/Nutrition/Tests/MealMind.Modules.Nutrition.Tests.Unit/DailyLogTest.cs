using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Exception;

namespace MealMind.Modules.Nutrition.Tests.Unit;

public class DailyLogTest
{
    [Test]
    public async Task AddMeal_ValidData_ShouldAddMeal()
    {
        var userId = Guid.NewGuid();

        var dailyLog = DailyLog.Create(DateOnly.FromDateTime(DateTime.Now), 70, 2000, userId);
        var meal = Meal.Initialize(MealType.Breakfast, userId);

        dailyLog.AddMeal(meal);

        await Assert.That(dailyLog.Meals).IsNotEmpty();
        await Assert.That(dailyLog.Meals[0].MealType).IsEqualTo(meal.MealType);
    }

    [Test]
    public async Task AddMeal_DuplicateMealType_ShouldThrow()
    {
        var userId = Guid.NewGuid();

        var dailyLog = DailyLog.Create(DateOnly.FromDateTime(DateTime.Now), 70, 2000, userId);
        var meal = Meal.Initialize(MealType.Breakfast, userId);
        var meal2 = Meal.Initialize(MealType.Breakfast, userId);

        dailyLog.AddMeal(meal);

        await Assert.That(() => dailyLog.AddMeal(meal2))
            .Throws<DomainException>()
            .WithMessage($"Meal of type {meal.MealType} already exists for this day.");
    }

    [Test]
    public async Task UpdateCurrentWeight_ValidData_ShouldUpdateWeight()
    {
        var dailyLog = DailyLog.Create(DateOnly.FromDateTime(DateTime.Now), 70, 2000, Guid.NewGuid());

        dailyLog.UpdateCurrentWeight(75);

        await Assert.That(dailyLog.CurrentWeight).IsEqualTo(75);
    }

    [Test]
    public async Task UpdateCurrentWeight_NegativeWeight_ShouldThrow()
    {
        var dailyLog = DailyLog.Create(DateOnly.FromDateTime(DateTime.Now), 70, 2000, Guid.NewGuid());

        await Assert.That(() => dailyLog.UpdateCurrentWeight(-75))
            .Throws<DomainException>()
            .WithMessage("Current weight cannot be negative.");
    }
}