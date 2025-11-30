using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.BackgroundJobs;

public class DailyLogBackgroundJob : BackgroundService
{
    private readonly ILogger<DailyLogBackgroundJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromHours(12));


    public DailyLogBackgroundJob(ILogger<DailyLogBackgroundJob> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Worker running on: {Name}, at: {Time}", nameof(DailyLogBackgroundJob), DateTime.UtcNow);
                await ProcessDailyLogsAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred executing {Name} at {Time}", nameof(DailyLogBackgroundJob), DateTime.UtcNow);
            }
        }
    }

    private async Task ProcessDailyLogsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<INutritionDbContext>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var filteredUserWithCount = await dbContext.DailyLogs
            .GroupBy(x => x.UserId)
            .Select(x => new
            {
                UserId = x.Key,
                FutureDailyLogCount = x.Count(z => z.CurrentDate >= today),
                LatestDailyLogDate = x.Max(z => z.CurrentDate)
            })
            .Where(x => x.FutureDailyLogCount < 90)
            .ToListAsync(cancellationToken);

        var userIds = filteredUserWithCount.Select(x => x.UserId).ToList();

        var users = await dbContext.UserProfiles
            .Include(x => x.NutritionTargets)
            .ThenInclude(x => x.ActiveDays)
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var userWithCount in filteredUserWithCount)
        {
            var userData = users.First(x => x.Id == userWithCount.UserId);
            var lastLog = userWithCount.LatestDailyLogDate;

            var logsToCreate = 90 - userWithCount.FutureDailyLogCount;

            for (var i = 0; i < logsToCreate; i++)
            {
                var newDate = lastLog.AddDays(i + 1);
                var caloriesGoalForDailyLog = userData.NutritionTargets
                    .FirstOrDefault(x => x.ActiveDays
                        .Any(z => z.DayOfWeek == newDate.DayOfWeek))!.Calories;

                var dailyLog = DailyLog.Create(newDate, null, caloriesGoalForDailyLog, userData.Id);

                foreach (var mealType in Enum.GetValues<MealType>())
                {
                    var meal = Meal.Initialize(mealType, userData.Id);
                    dailyLog.AddMeal(meal);
                }

                await dbContext.DailyLogs.AddAsync(dailyLog, cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    // await Parallel.ForEachAsync(filteredUserWithCount, new  ParallelOptions { MaxDegreeOfParallelism = 5 }, async ( userWithCount, token) =>
    // {
    //     var userData = users.First(x => x.Id == userWithCount.UserId);
    //     var lastLog = userWithCount.LatestDailyLogDate;
    //
    //     var logsToCreate = 90 - userWithCount.FutureDailyLogCount;
    //
    //     for (var i = 0; i < logsToCreate; i++)
    //     {
    //         var newDate = lastLog.AddDays(i + 1);
    //         var caloriesGoalForDailyLog = userData.NutritionTargets
    //             .FirstOrDefault(x => x.ActiveDays
    //                 .Any(z => z.DayOfWeek == newDate.DayOfWeek))!.Calories;
    //
    //         var dailyLog = DailyLog.Create(newDate, null, caloriesGoalForDailyLog, userData.Id);
    //
    //         foreach (var mealType in Enum.GetValues<MealType>())
    //         {
    //             var meal = Meal.Initialize(mealType, userData.Id);
    //             dailyLog.AddMeal(meal);
    //         }
    //
    //         await dbContext.DailyLogs.AddAsync(dailyLog, token);
    //     }
    // });
}