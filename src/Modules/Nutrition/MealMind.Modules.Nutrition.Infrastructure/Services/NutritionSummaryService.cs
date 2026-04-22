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

    public async Task<string> BuildSummaryAsync(UserId userId, CancellationToken ct)
    {
        var user = await _dbContext.UserProfiles.FindAsync([userId], ct);
        if (user == null)
            throw new DomainException("User not found.");

        var days = user.SubscriptionTier switch
        {
            SubscriptionTier.Standard => 7,
            SubscriptionTier.Premium => 30,
            _ => throw new ArgumentOutOfRangeException(nameof(user.SubscriptionTier), "Invalid subscription tier.")
        };

        var dailyLogs = await _dbContext.DailyLogs
            .Where(x => x.UserId == userId &&
                x.CurrentDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days))
            )
            .OrderByDescending(x => x.CurrentDate)
            .ToListAsync(ct);

        var firstWeightWithValue = dailyLogs.First(x => x.CurrentWeight.HasValue).CurrentWeight;
        var lastWeightWithValue = dailyLogs.Last(x => x.CurrentWeight.HasValue).CurrentWeight;

        var summaryBuilder = new StringBuilder();
        summaryBuilder.AppendLine($"Nutrition Summary for {user.Username}:");
        summaryBuilder.AppendLine($"User weight target: {user.PersonalData.WeightTarget} kg");
        summaryBuilder.AppendLine($"## Overall ({days} days):)");
        summaryBuilder.AppendLine($"- Calories: {dailyLogs.Sum(x => x.TotalCalories) / days} kcal avg");
        summaryBuilder.AppendLine($"- Weight: " +
            $"{firstWeightWithValue} -> {lastWeightWithValue} ({(lastWeightWithValue - firstWeightWithValue) / firstWeightWithValue * 100:0.00}%)");

        return summaryBuilder.ToString();
    }
}