using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Pagination;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Application.Features.Queries.GetFoodByNameQuery;

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
                    x => EF.Functions.ILike(x.Name, $"%{query.SearchTerm}%") ||
                        EF.Functions.ILike(x.Brand!, $"%{query.SearchTerm}%"))
                .Include(x => x.Statistics)
                .OrderByDescending(x => x.Statistics.TotalUsageCount * 0.5 + x.Statistics.FavoriteCount * 2 + x.Statistics.SearchCount * 0.05)
                .ThenByDescending(x => x.Statistics.AverageRating * x.Statistics.RatingCount / (x.Statistics.RatingCount + 5.0))
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize + 1)
                .ToListAsync(cancellationToken);

            var foodsDto = databaseFoods
                .Select(x => new FoodDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Barcode = x.Barcode,
                    Brand = x.Brand,
                    ImageUrl = x.ImageUrl,
                    NutritionPer100G = new NutrimentsPer100GDto
                    {
                        Calories = x.NutritionPer100G.Calories,
                        Protein = x.NutritionPer100G.Protein,
                        Carbohydrates = x.NutritionPer100G.Carbohydrates,
                        Fat = x.NutritionPer100G.Fat,
                        Fiber = x.NutritionPer100G.Fiber,
                        Sugar = x.NutritionPer100G.Sugar,
                        SaturatedFat = x.NutritionPer100G.SaturatedFat,
                        Sodium = x.NutritionPer100G.Sodium,
                        Salt = x.NutritionPer100G.Salt,
                        Cholesterol = x.NutritionPer100G.Cholesterol
                    },
                    CreatedAt = x.CreatedAt,
                    FoodSource = x.FoodDataSource.ToString(),
                })
                .ToList();

            if (foodsDto.Count > query.PageSize)
                return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, foodsDto.Count, foodsDto.Take(query.PageSize).ToList());

            if (foodsDto.Count < query.PageSize && foodsDto.Count != 0)
            {
                var needed = query.PageSize - foodsDto.Count;

                var externalFoods = await _openFoodFactsService
                    .SearchFoodByNameWithoutDuplicatesAsync(query.SearchTerm, needed, 1, foodsDto, cancellationToken);

                var externalFoodsDto = externalFoods
                    .Select(x => new FoodDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Barcode = x.Barcode,
                        FoodSource = x.FoodSource
                    })
                    .ToList();

                var combinedFoods = foodsDto
                    .Concat(externalFoodsDto)
                    .Take(query.PageSize)
                    .ToList();

                return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, combinedFoods.Count, combinedFoods);
            }

            var foods = await _openFoodFactsService
                .SearchFoodByNameAsync(query.SearchTerm, query.PageSize, 1, cancellationToken);

            var foodsMapped = foods
                .Select(x => new FoodDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Barcode = x.Barcode,
                    Brand = x.Brand,
                    FoodSource = x.FoodSource
                })
                .ToList();

            return PaginatedList<FoodDto>.Create(query.Page, query.PageSize, foods.Count, foodsMapped);
        }
    }
}