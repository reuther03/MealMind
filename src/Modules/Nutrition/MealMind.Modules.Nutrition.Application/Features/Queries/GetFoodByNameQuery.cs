using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Application.Dtos;
using MealMind.Shared.Abstractions.Kernel.Pagination;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Application.Features.Queries;

public record GetFoodByNameQuery(string SearchTerm, int PageSize = 10, int Page = 1) : IQuery<PaginatedList<FoodDto>>
{
    public sealed class Handler : IQueryHandler<GetFoodByNameQuery, PaginatedList<FoodDto>>
    {
        private readonly INutritionDbContext _context;
        private readonly IOpenFoodFactsService _openFoodFactsService;

        public Handler(INutritionDbContext context, IOpenFoodFactsService openFoodFactsService)
        {
            _context = context;
            _openFoodFactsService = openFoodFactsService;
        }

        public async Task<Result<PaginatedList<FoodDto>>> Handle(GetFoodByNameQuery query, CancellationToken cancellationToken = default)
        {
            var databaseFoods = await _context.Foods
                .WhereIf(!string.IsNullOrWhiteSpace(query.SearchTerm),
                    x => EF.Functions.Like(x.Name, $"%{query.SearchTerm}%") ||
                        EF.Functions.Like(x.Brand, $"%{query.SearchTerm}%"))
                .Join(_context.FoodStatistics,
                    food => food.Id,
                    stats => stats.FoodId,
                    (food, stats) => new { Food = food, Stats = stats })
                .OrderByDescending(x => x.Stats.TotalUsageCount * 0.5 + x.Stats.FavoriteCount * 2 + x.Stats.SearchCount * 0.05)
                .ThenByDescending(x => x.Stats.AverageRating * x.Stats.RatingCount / (x.Stats.RatingCount + 5.0))
                .Select(x => x.Food)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize + 1)
                .ToListAsync(cancellationToken);

            var foodsDto = databaseFoods
                .Select(FoodDto.AsDto)
                .ToList();

            if (foodsDto.Count > query.PageSize)
                return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, foodsDto.Count, foodsDto.Take(query.PageSize).ToList());

            if (foodsDto.Count < query.PageSize && foodsDto.Count != 0)
            {
                var needed = query.PageSize - foodsDto.Count;

                var externalFoods = await _openFoodFactsService
                    .SearchFoodByNameWithoutDuplicatesAsync(query.SearchTerm, needed, 1, foodsDto, cancellationToken);

                var combinedFoods = foodsDto
                    .Concat(externalFoods)
                    .Take(query.PageSize)
                    .ToList();

                return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, combinedFoods.Count, combinedFoods);
            }

            var foods = await _openFoodFactsService
                .SearchFoodByNameAsync(query.SearchTerm, query.PageSize, 1, cancellationToken);

            return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, foods.Count, foods);
        }
    }
}