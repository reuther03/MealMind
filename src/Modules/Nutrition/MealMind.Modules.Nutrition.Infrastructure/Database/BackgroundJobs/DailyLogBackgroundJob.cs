using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.BackgroundJobs;

public class DailyLogBackgroundJob : BackgroundService
{
    private readonly ILogger<DailyLogBackgroundJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMinutes(20));


    public DailyLogBackgroundJob(ILogger<DailyLogBackgroundJob> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

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
                throw;
            }
        }
    }

    private async Task ProcessDailyLogsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<INutritionDbContext>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // var filteredUsers = await dbContext.DailyLogs.Where(x => x.CurrentDate >= today)

        await Task.CompletedTask;
    }
}