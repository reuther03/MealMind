using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Application.Dtos;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Pagination;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;

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
            var foods = await _openFoodFactsService.SearchFoodByNameAsync(query.SearchTerm, query.PageSize, query.Page, cancellationToken);


            return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, foods.Count, foods);
        }
    }
}