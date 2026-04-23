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

    public async Task<string> BuildSummaryAsync(UserId userId, int? weeks, CancellationToken ct)
    {
        if (weeks is < 1 or > 4)
            throw new DomainException("Weeks must be between 1 and 4.");

        var user = await _dbContext.UserProfiles
            .Include(x => x.PersonalData)
            .Include(x => x.NutritionTargets)
            .ThenInclude(nutritionTarget => nutritionTarget.ActiveDays)
            .FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null)
            throw new DomainException("User not found.");

        var days = weeks * 7;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var mondayThisWeek = today.AddDays(-daysFromMonday);
        var sundayLastWeek = mondayThisWeek.AddDays(-1);
        var mondayRangeStart = mondayThisWeek.AddDays((int)(-7 * weeks)!);

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

        var firstWeight = dailyLogs.FirstOrDefault(x => x.CurrentWeight.HasValue)?.CurrentWeight;
        var lastWeight = dailyLogs.LastOrDefault(x => x.CurrentWeight.HasValue)?.CurrentWeight;
        var avgCal = dailyLogs.Count == 0 ? 0 : dailyLogs.Sum(x => x.TotalCalories) / dailyLogs.Count;

        var summaryBuilder = new StringBuilder();
        summaryBuilder.AppendLine($"Nutrition Summary for {user.Username}:");
        summaryBuilder.AppendLine("Week summary monday to sunday");
        summaryBuilder.AppendLine($"User weight target: {user.PersonalData.WeightTarget} kg");
        summaryBuilder.AppendLine($"## Overall ({days} days):");
        summaryBuilder.AppendLine($"- Calories: {avgCal} kcal avg");
        if (firstWeight.HasValue && lastWeight.HasValue && firstWeight.Value != 0)
        {
            var pct = (lastWeight.Value - firstWeight.Value) / firstWeight.Value * 100;
            summaryBuilder.AppendLine($"- Weight: {firstWeight} -> {lastWeight} ({pct:0.00}%)");
        }
        else
        {
            summaryBuilder.AppendLine("- Weight: not tracked in this period");
        }

        summaryBuilder.AppendLine($"{dailyLogs.Count} days logged of {days} days.");

        var targetByDay = user.NutritionTargets
            .SelectMany(t => t.ActiveDays
                .Select(d => (Day: d.DayOfWeek, Target: t)))
            .ToDictionary(x => x.Day, x => x.Target);

        foreach (var group in weeklyGroups)
        {
            var weekLogs = group.ToList();
            if (weekLogs.Count == 0) continue;
            var daysOnTarget = weekLogs.Count(x =>
                targetByDay.TryGetValue(x.CurrentDate.DayOfWeek, out var t)
                && Math.Abs(x.TotalCalories - t.Calories) <= t.Calories * 0.1m);

            var weightsInWeek = weekLogs.Where(x => x.CurrentWeight.HasValue)
                .Select(x => x.CurrentWeight!.Value)
                .ToList();

            summaryBuilder.AppendLine($"## Week of {group.Key:yyyy-MM-dd}:");
            summaryBuilder.AppendLine($"- Calories: {weekLogs.Sum(x => x.TotalCalories) / weekLogs.Count} kcal avg");
            var weightText = weightsInWeek.Count == 0
                ? "N/A"
                : $"{weightsInWeek.Average():F1} kg";
            summaryBuilder.AppendLine($"- Weight (week avg): {weightText}");
            summaryBuilder.AppendLine($"{weekLogs.Count} days logged of 7 days.");
            summaryBuilder.AppendLine($"- On target: {daysOnTarget}/{weekLogs.Count} days ({daysOnTarget * 100 / weekLogs.Count}%)");
        }

        return summaryBuilder.ToString();
    }
}