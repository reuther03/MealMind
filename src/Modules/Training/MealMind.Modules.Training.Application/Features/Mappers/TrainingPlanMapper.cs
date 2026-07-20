using System.Linq.Expressions;
using LinqKit;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Contracts.Dto.Training;

namespace MealMind.Modules.Training.Application.Features.Mappers;

public static class TrainingPlanMapper
{
    public static Expression<Func<TrainingPlan, TrainingPlanDetailsDto>> DetailsProjection { get; } =
        entity => new TrainingPlanDetailsDto
        {
            Id = entity.Id,
            Name = entity.Name,
            PlannedOn = entity.PlannedOn,
            IsActive = entity.IsActive,
            Sessions = entity.Sessions
                .AsQueryable()
                .Select(ts => TrainingSessionMapper.Projection.Invoke(ts))
                .ToList()
        };
}
