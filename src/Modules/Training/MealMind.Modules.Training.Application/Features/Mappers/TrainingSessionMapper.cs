using System.Linq.Expressions;
using LinqKit;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Contracts.Dto.Training;

namespace MealMind.Modules.Training.Application.Features.Mappers;

public static class TrainingSessionMapper
{
    public static Expression<Func<TrainingSession, TrainingSessionDto>> Projection { get; } =
        entity => new TrainingSessionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            Description = entity.Description,
            ExerciseNames = entity.Exercises
                .OrderBy(e => e.OrderIndex)
                .Select(e => e.Exercise.Name)
                .ToList()
        };

    public static Expression<Func<TrainingSession, TrainingSessionDetailsDto>> DetailsProjection { get; } =
        entity => new TrainingSessionDetailsDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            Exercises = entity.Exercises
                .AsQueryable()
                .OrderBy(e => e.OrderIndex)
                .Select(e => ExerciseMapper.SessionExerciseProjection.Invoke(e))
                .ToList(),
        };
}
