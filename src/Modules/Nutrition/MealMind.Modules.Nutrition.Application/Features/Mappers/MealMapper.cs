using System.Linq.Expressions;
using LinqKit;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Nutrition.Application.Features.Mappers;

public static class MealMapper
{
    public static Expression<Func<Meal, MealDto>> Projection { get; }
        = entity => new MealDto
        {
            Id = entity.Id,
            MealType = (int)entity.MealType,
            Name = entity.Name!.Value,
            Foods = entity.Foods.AsQueryable()
                .Select(f => FoodEntryMapper.Projection.Invoke(f))
                .ToList()
        };
}
