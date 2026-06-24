using System.Text;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Infrastructure.Database.Services;

internal class TrainingSummaryService : ITrainingSummaryService
{
    private readonly TrainingDbContext _dbContext;

    public TrainingSummaryService(TrainingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> BuildSummaryAsync(UserId userId, CancellationToken ct)
    {
        var days = 14;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var mondayThisWeek = today.AddDays(-daysFromMonday);
        var sundayLastWeek = mondayThisWeek.AddDays(-1);
        var mondayRangeStart = mondayThisWeek.AddDays(-1 * days);


        var trainingPlans = await _dbContext.TrainingPlans
            .SelectMany(x => x.Sessions)
            .Where(s => s.EndedAt != null
                && s.EndedAt.Value >= mondayRangeStart.ToDateTime(TimeOnly.MinValue)
                && s.EndedAt.Value < sundayLastWeek.AddDays(1).ToDateTime(TimeOnly.MinValue))
            .OrderBy(s => s.EndedAt)
            .Select(s => new
            {
                s.Name,
                s.StartedAt,
                s.EndedAt,
                s.Description,
                Exercises = s.Exercises.Select(e => new
                {
                    e.Exercise.Name,
                    e.Exercise.Type,
                    e.Exercise.MuscleGroup,
                    e.OrderIndex,
                    e.StrengthDetails,
                    e.CardioDetails
                }).ToList()
            }).ToListAsync(ct);

        if (trainingPlans.Count == 0)
            throw new DomainException("No training sessions found in the specified period.");

        var trainingPlansWeeklyGroup = trainingPlans
            .GroupBy(x => x.StartedAt!.Value.AddDays(((int)today.DayOfWeek + 6) % 7))
            .OrderBy(g => g.Key)
            .ToList();

        var summaryBuilder = new StringBuilder();
        summaryBuilder.AppendLine("Training Summary");
        foreach (var plan in trainingPlans)
        {
        }


        return "xd";
    }
}