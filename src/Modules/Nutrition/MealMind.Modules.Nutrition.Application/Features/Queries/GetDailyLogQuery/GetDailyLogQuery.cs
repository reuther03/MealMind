using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Application.Features.Queries.GetDailyLogQuery;

public record GetDailyLogQuery(DateOnly? DailyLogDate) : IQuery<DailyLogDto>
{
    public sealed class Handler : IQueryHandler<GetDailyLogQuery, DailyLogDto>
    {
        private readonly INutritionDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(INutritionDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<DailyLogDto>> Handle(GetDailyLogQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.UserProfiles.FindAsync([_userService.UserId], cancellationToken);
            if (user is null)
                return Result<DailyLogDto>.BadRequest("User not found.");

            var currentDate = query.DailyLogDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var dailyLogDto = await _dbContext.DailyLogs
                .Where(x => x.UserId == user.Id && x.CurrentDate == currentDate)
                .Select(x => new DailyLogDto
                {
                    CurrentDate = x.CurrentDate,
                    CurrentWeight = x.CurrentWeight,
                    CaloriesGoal = x.CaloriesGoal,
                    UserId = x.UserId.Value,
                    Meals = x.Meals
                        .OrderBy(m => (int)m.MealType)
                        .Select(m => new MealDto
                        {
                            MealType = (int)m.MealType,
                            Name = m.Name!.Value,
                            Foods = m.Foods.Select(f => new FoodEntryDto
                            {
                                FoodId = f.FoodId != null ? f.FoodId.Value : null,
                                FoodName = f.FoodName.Value,
                                FoodBrand = f.FoodBrand != null ? f.FoodBrand.Value : null,
                                QuantityInGrams = f.QuantityInGrams,
                                TotalCalories = f.TotalCalories,
                                TotalProteins = f.TotalProteins,
                                TotalCarbohydrates = f.TotalCarbohydrates,
                                TotalSugars = f.TotalSugars,
                                TotalFats = f.TotalFats,
                                TotalSaturatedFats = f.TotalSaturatedFats,
                                TotalFiber = f.TotalFiber,
                                TotalSodium = f.TotalSodium,
                                TotalSalt = f.TotalSalt,
                                TotalCholesterol = f.TotalCholesterol,
                                Source = f.Source.ToString()
                            }).ToList()
                        }).ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);

            return dailyLogDto is null
                ? Result<DailyLogDto>.BadRequest("Daily log not found for the specified date.")
                : Result<DailyLogDto>.Ok(dailyLogDto);
        }
    }
}