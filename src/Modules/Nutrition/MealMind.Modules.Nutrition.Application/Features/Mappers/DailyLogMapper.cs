using System.Linq.Expressions;
using LinqKit;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Nutrition.Application.Features.Mappers;

public static class DailyLogMapper
{
    public static Expression<Func<DailyLog, DailyLogDto>> Projection { get; }
        = entity => new DailyLogDto
        {
            Id = entity.Id,
            CurrentDate = entity.CurrentDate,
            CurrentWeight = entity.CurrentWeight,
            CaloriesGoal = entity.CaloriesGoal,
            UserId = entity.UserId.Value,
            Meals = entity.Meals.AsQueryable()
                //todo: wyniesc filtry do query
                .OrderBy(m => (int)m.MealType)
                .Select(m => MealMapper.Projection.Invoke(m))
                .ToList()
        };
}
