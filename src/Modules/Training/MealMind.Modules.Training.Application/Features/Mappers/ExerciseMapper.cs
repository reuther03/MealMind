using System.Linq.Expressions;
using LinqKit;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Contracts.Dto.Training;

namespace MealMind.Modules.Training.Application.Features.Mappers;

public static class ExerciseMapper
{
    // public static Expression<Func<TrainingPlan, TrainingPlanDto>> Projection
    //     = entity => new TrainingPlanDto
    //     {
    //         Id = entity.Id,
    //         Name = entity.Name,
    //         PlannedOn = entity.PlannedOn,
    //         IsActive = entity.IsActive,
    //         SessionsCount = entity.Sessions.Count,
    //         LastCompletedSessionAt = entity.Sessions
    //             .Where(s => s.EndedAt.HasValue)
    //             .OrderByDescending(s => s.EndedAt)
    //             .Select(s => s.EndedAt)
    //             .FirstOrDefault()
    //     };

    public static Expression<Func<Exercise, ExerciseDto>> ExerciseProjection { get; }
        = entity => new ExerciseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.ImageUrl,
            Type = entity.Type.ToString(),
            IsCustom = entity.IsCustom,
            MuscleGroup = entity.MuscleGroup != null ? entity.MuscleGroup.ToString() : null
        };

    public static Expression<Func<SessionExercise, SessionExerciseDto>> SessionExerciseProjection { get; }
        = entity =>
        new SessionExerciseDto
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            Exercise = ExerciseProjection.Invoke(entity.Exercise),
            StrengthDetails = entity.StrengthDetails == null
                ? null
                : StrengthDetailsProjection.Invoke(entity.StrengthDetails),

            CardioDetails = entity.CardioDetails == null
                ? null
                : CardioDetailsProjection.Invoke(entity.CardioDetails)
        };


    public static Expression<Func<StrengthDetails, StrengthDetailsDto>> StrengthDetailsProjection { get; }
        = entity => new StrengthDetailsDto
        {
            ExerciseSets = entity.Sets
                .AsQueryable()
                .Select(es => ExerciseSetProjection.Invoke(es))
                .ToList()
        };

    public static Expression<Func<ExerciseSet, ExerciseSetDto>> ExerciseSetProjection { get; }
        = entity => new ExerciseSetDto
        {
            SetNumber = entity.SetNumber,
            Repetitions = entity.Repetitions,
            Weight = entity.Weight,
            SetType = entity.SetType.ToString(),
            RestTimeInSeconds = entity.RestTimeInSeconds
        };

    public static Expression<Func<CardioDetails, CardioDetailsDto>> CardioDetailsProjection { get; }
        = entity => new CardioDetailsDto
        {
            DurationInMinutes = entity.DurationInMinutes,
            DistanceInKm = entity.DistanceInKm,
            CaloriesBurned = entity.CaloriesBurned,
            AverageHeartRate = entity.AverageHeartRate,
            AverageSpeed = entity.AverageSpeed,
            Notes = entity.Notes,
            CaloriesEstimated = entity.CaloriesEstimated
        };
}
