using MealMind.Modules.AiChat.Application.Abstractions.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.AiChat.Infrastructure.Database.BackgroundJobs;

public class DeleteImageAnalyzeJob : BackgroundService
{
    private readonly ILogger<DeleteImageAnalyzeJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromDays(1));

    public DeleteImageAnalyzeJob(ILogger<DeleteImageAnalyzeJob> logger, IServiceScopeFactory serviceScopeFactory)
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
                _logger.LogInformation("Worker running on: {Name}, at: {Time}", nameof(DeleteImageAnalyzeJob), DateTime.UtcNow);
                await ProcessDeleteImageAnalyzeAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred executing {Name} at {Time}", nameof(DeleteImageAnalyzeJob), DateTime.UtcNow);
            }
        }
    }

    private async Task ProcessDeleteImageAnalyzeAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAiChatDbContext>();

        var today = DateTime.UtcNow;

        var imageAnalyzesToDelete = await dbContext.FoodImageAnalyzes
            .Where(x =>
                (x.SavedAt != null && x.SavedAt.Value <= today.AddDays(-7)) ||
                (x.SavedAt == null && x.CreatedAt <= today.AddDays(-14))
            ).ToListAsync(cancellationToken);

        if (imageAnalyzesToDelete.Count != 0)
        {
            dbContext.FoodImageAnalyzes.RemoveRange(imageAnalyzesToDelete);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} FoodImageAnalyzes records", imageAnalyzesToDelete.Count);
        }
    }
}