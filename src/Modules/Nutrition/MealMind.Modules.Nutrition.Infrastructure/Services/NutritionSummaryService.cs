using System.Text;
using MealMind.Modules.Nutrition.Infrastructure.Database;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Infrastructure.Services;

internal class NutritionSummaryService : INutritionSummaryService
{
    private readonly NutritionDbContext _dbContext;

    public NutritionSummaryService(NutritionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> BuildSummaryAsync(UserId userId, int weeks, CancellationToken ct)
    {
        if (weeks is < 1 or > 4)
            throw new DomainException("Weeks must be between 1 and 4.", nameof(weeks));

        var user = await _dbContext.UserProfiles.FindAsync([userId], ct);
        if (user == null)
            throw new DomainException("User not found.");

        var days = weeks * 7;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var mondayThisWeek = today.AddDays(-daysFromMonday);
        var sundayLastWeek = mondayThisWeek.AddDays(-1);
        var mondayRangeStart = mondayThisWeek.AddDays(-7 * weeks);

        var dailyLogs = await _dbContext.DailyLogs
            .Where(x => x.UserId == userId
                && x.CurrentDate >= mondayRangeStart
                && x.CurrentDate <= sundayLastWeek)
            .OrderBy(x => x.CurrentDate)
            .ToListAsync(ct);

        var weeklyGroups = dailyLogs
            .GroupBy(x => x.CurrentDate.AddDays(-(((int)x.CurrentDate.DayOfWeek + 6) % 7)))
            .OrderBy(g => g.Key)
            .ToList();

        var firstWeightWithValue = dailyLogs.First(x => x.CurrentWeight.HasValue).CurrentWeight;
        var lastWeightWithValue = dailyLogs.Last(x => x.CurrentWeight.HasValue).CurrentWeight;

        // int daysOnTarget = dailyLogs.Count(x =>
        //     Math.Abs(x.TotalCalories - user.NutritionTargets.FirstOrDefault(z => z.ActiveDays.Contains(x.CurrentDate.DayOfWeek))?.Calories ?? 0)
        //     <= user.Target.Calories * 0.1);

        var summaryBuilder = new StringBuilder();
        summaryBuilder.AppendLine($"Nutrition Summary for {user.Username}:");
        summaryBuilder.AppendLine("Week summary monday to sunday");
        summaryBuilder.AppendLine($"User weight target: {user.PersonalData.WeightTarget} kg");
        summaryBuilder.AppendLine($"## Overall ({days} days):)");
        summaryBuilder.AppendLine($"- Calories: {dailyLogs.Sum(x => x.TotalCalories) / days} kcal avg");
        summaryBuilder.AppendLine($"- Weight: " +
            $"{firstWeightWithValue} -> {lastWeightWithValue} ({(lastWeightWithValue - firstWeightWithValue) / firstWeightWithValue * 100:0.00}%)");
        summaryBuilder.AppendLine($"{dailyLogs.Count} days logged of {days} days.");


        return summaryBuilder.ToString();
    }
}