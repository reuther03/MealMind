using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Seeders;

public class NutritionModuleSeeder : IModuleSeeder
{
    private const int WeeksToSeed = 4;
    private const decimal StartingWeightKg = 82.5m;
    private const decimal DailyWeightDriftKg = 0.05m;

    private readonly INutritionDbContext _dbContext;

    public NutritionModuleSeeder(INutritionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var mondayThisWeek = today.AddDays(-daysFromMonday);
        var sundayLastWeek = mondayThisWeek.AddDays(-1);
        var rangeStart = mondayThisWeek.AddDays(-7 * WeeksToSeed);

        var users = await _dbContext.UserProfiles
            .Include(x => x.NutritionTargets)
            .ThenInclude(t => t.ActiveDays)
            .ToListAsync(cancellationToken);

        var random = new Random(42);

        foreach (var user in users)
        {
            if (user.NutritionTargets.Count == 0)
                continue;

            var existingDates = await _dbContext.DailyLogs
                .Where(x => x.UserId == user.Id
                    && x.CurrentDate >= rangeStart
                    && x.CurrentDate <= sundayLastWeek)
                .Select(x => x.CurrentDate)
                .ToListAsync(cancellationToken);

            var existingDateSet = existingDates.ToHashSet();

            var weight = StartingWeightKg;

            for (var date = rangeStart; date <= sundayLastWeek; date = date.AddDays(1))
            {
                if (existingDateSet.Contains(date))
                    continue;

                var target = user.NutritionTargets
                    .FirstOrDefault(t => t.ActiveDays.Any(d => d.DayOfWeek == date.DayOfWeek));

                if (target is null)
                    continue;

                var jitter = (decimal)(random.NextDouble() * 0.30 - 0.15);
                var calories = Math.Round(target.Calories * (1 + jitter), 2);

                decimal? logWeight = date.DayNumber % 2 == 0
                    ? Math.Round(weight, 2)
                    : null;
                weight -= DailyWeightDriftKg;

                var dailyLog = DailyLog.Create(date, logWeight, target.Calories, user.Id);

                var meal = Meal.Initialize(MealType.Breakfast, user.Id);
                var food = FoodEntry.CreateFromImageAnalyze(
                    foodName: new Name("Seed Food"),
                    quantityInGrams: 500m,
                    totalCalories: calories,
                    totalProteins: 200m,
                    totalCarbohydrates: 400m,
                    totalFats: 100m);
                meal.AddFood(food);
                dailyLog.AddMeal(meal);

                await _dbContext.DailyLogs.AddAsync(dailyLog, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}