using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Contracts.Dto.Training;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Infrastructure.Database.Services;

internal class SessionComparisonService : ISessionComparisonService
{
    private readonly TrainingDbContext _dbContext;

    public SessionComparisonService(TrainingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SessionComparisonDto> CompareSessionsAsync(Guid currentSessionId, CancellationToken cancellationToken)
    {
        var sessionsToComparison = await _dbContext.TrainingPlans
            .Include(x => x.Sessions)
            .ThenInclude(x => x.Exercises)
            .Where(x => x.Sessions.Any(s => s.Id == currentSessionId))
            .Select(x => new
            {
                CurrentSession = x.Sessions.FirstOrDefault(s => s.Id == currentSessionId),
                PreviousSession = x.Sessions
                    .Where(s => s.IsCompleted && s.Id != currentSessionId)
                    .OrderByDescending(s => s.EndedAt)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (sessionsToComparison?.CurrentSession is null)
            throw new DomainException("Current session not found.");


        var sessionsToComparisonDto = new SessionComparisonDto
        {
            SessionId = sessionsToComparison.CurrentSession.Id,
            PreviousSessionId = sessionsToComparison.PreviousSession?.Id,
            EndedAt = sessionsToComparison.CurrentSession.EndedAt!.Value,
            PreviousEndedAt = sessionsToComparison.PreviousSession?.EndedAt,
            Current = new SessionStatsDto
            {
                ExercisesCount = sessionsToComparison.CurrentSession.Exercises.Count,
                TotalSets = sessionsToComparison.CurrentSession.Exercises.Sum(x => x.StrengthDetails?.Sets.Count ?? 0),
                TotalVolume = sessionsToComparison.CurrentSession.Exercises.Sum(x => x.StrengthDetails?.Sets.Sum(s => s.Weight * s.Repetitions) ?? 0),
                CardioDurationInMinutes = sessionsToComparison.CurrentSession.Exercises.Sum(x => x.CardioDetails?.DurationInMinutes ?? 0),
                CardioCaloriesBurned = sessionsToComparison.CurrentSession.Exercises.Sum(x => x.CardioDetails?.CaloriesBurned ?? 0)
            },
            Previous = sessionsToComparison.PreviousSession is not null
                ? new SessionStatsDto
                {
                    ExercisesCount = sessionsToComparison.PreviousSession.Exercises.Count,
                    TotalSets = sessionsToComparison.PreviousSession.Exercises.Sum(x => x.StrengthDetails?.Sets.Count ?? 0),
                    TotalVolume = sessionsToComparison.PreviousSession.Exercises.Sum(x => x.StrengthDetails?.Sets.Sum(s => s.Weight * s.Repetitions) ?? 0),
                    CardioDurationInMinutes = sessionsToComparison.PreviousSession.Exercises.Sum(x => x.CardioDetails?.DurationInMinutes ?? 0),
                    CardioCaloriesBurned = sessionsToComparison.PreviousSession.Exercises.Sum(x => x.CardioDetails?.CaloriesBurned ?? 0)
                }
                : null,
            Exercises = sessionsToComparison.CurrentSession.Exercises.Select(currentExercise =>
            {
                var previousExercise = sessionsToComparison.PreviousSession?.Exercises.FirstOrDefault(e => e.Exercise.Id == currentExercise.Exercise.Id);
                return new ExerciseComparisonDto
                {
                    ExerciseName = currentExercise.Exercise.Name,
                    ExerciseType = currentExercise.Exercise.Type.ToString(),
                    CurrentBestWeight = currentExercise.StrengthDetails?.Sets.Max(s => s.Weight),
                    CurrentBestReps = currentExercise.StrengthDetails?.Sets.Max(s => s.Repetitions),
                    PreviousBestWeight = previousExercise?.StrengthDetails?.Sets.Max(s => s.Weight),
                    PreviousBestReps = previousExercise?.StrengthDetails?.Sets.Max(s => s.Repetitions),
                    WeightDelta = previousExercise is not null && currentExercise.StrengthDetails is not null
                        ? currentExercise.StrengthDetails.Sets.Max(s => s.Weight) - previousExercise.StrengthDetails!.Sets.Max(s => s.Weight)
                        : null,
                    RepsDelta = previousExercise is not null && currentExercise.StrengthDetails is not null
                        ? currentExercise.StrengthDetails.Sets.Max(s => s.Repetitions) - previousExercise.StrengthDetails!.Sets.Max(s => s.Repetitions)
                        : null,
                    CurrentVolume = currentExercise.StrengthDetails?.Sets.Sum(s => s.Weight * s.Repetitions),
                    PreviousVolume = previousExercise?.StrengthDetails?.Sets.Sum(s => s.Weight * s.Repetitions),
                    VolumeDelta = previousExercise is not null && currentExercise.StrengthDetails is not null
                        ? currentExercise.StrengthDetails.Sets.Sum(s => s.Weight * s.Repetitions) -
                        previousExercise.StrengthDetails!.Sets.Sum(s => s.Weight * s.Repetitions)
                        : null,
                    CurrentDurationInMinutes = currentExercise.CardioDetails?.DurationInMinutes,
                    PreviousDurationInMinutes = previousExercise?.CardioDetails?.DurationInMinutes,
                    DurationDelta = previousExercise is not null && currentExercise.CardioDetails is not null
                        ? currentExercise.CardioDetails.DurationInMinutes - previousExercise.CardioDetails!.DurationInMinutes
                        : null,
                    CurrentDistanceInKm = currentExercise.CardioDetails?.DistanceInKm,
                    PreviousDistanceInKm = previousExercise?.CardioDetails?.DistanceInKm,
                    DistanceDelta = previousExercise is not null && currentExercise.CardioDetails is not null
                        ? currentExercise.CardioDetails.DistanceInKm - previousExercise.CardioDetails!.DistanceInKm
                        : null,
                    CurrentCaloriesBurned = currentExercise.CardioDetails?.CaloriesBurned,
                    PreviousCaloriesBurned = previousExercise?.CardioDetails?.CaloriesBurned,
                    CaloriesBurnedDelta = previousExercise is not null && currentExercise.CardioDetails is not null
                        ? currentExercise.CardioDetails.CaloriesBurned - previousExercise.CardioDetails!.CaloriesBurned
                        : null
                };
            }).ToList()
        };

        return sessionsToComparisonDto;
    }
}