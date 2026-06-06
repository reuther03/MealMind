using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Nutrition.Application.Features.Queries.GetDailyLogsRangeQuery;

public record GetDailyLogsRangeQuery(DateOnly StartDate, DateOnly EndDate) : IQuery<DailyLogsRangeDto>
{
    public sealed class Handler : IQueryHandler<GetDailyLogsRangeQuery, DailyLogsRangeDto>
    {
        private readonly INutritionDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(INutritionDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<DailyLogsRangeDto>> Handle(GetDailyLogsRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
                return Result<DailyLogsRangeDto>.BadRequest("Start date cannot be after end date.");

            var userId = _userService.UserId;

            var logs = await _dbContext.DailyLogs
                .Where(x => x.UserId == userId && x.CurrentDate >= request.StartDate && x.CurrentDate <= request.EndDate)
                .OrderBy(x => x.CurrentDate)
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
                            }).ToList()
                        }).ToList()
                }).ToListAsync(cancellationToken);

            if (logs.Count == 0)
                return Result<DailyLogsRangeDto>.Ok(new DailyLogsRangeDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDaysInRange = request.EndDate.DayNumber - request.StartDate.DayNumber + 1,
                    DaysLogged = 0,
                    DaysOnTarget = 0,
                    RangeTotals = new NutritionTotalsDto(),
                    DailyAverages = new NutritionAveragesDto(),
                    DailyLogs = []
                });

            var dto = new DailyLogsRangeDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DaysLogged = logs.Count,
                TotalDaysInRange = request.EndDate.DayNumber - request.StartDate.DayNumber + 1,
                DaysOnTarget = logs.Count(l => Math.Abs(l.TotalCalories - l.CaloriesGoal) <= l.CaloriesGoal * 0.1m),
                RangeTotals = new NutritionTotalsDto
                {
                    Calories = logs.Sum(l => l.TotalCalories),
                    Proteins = logs.Sum(l => l.TotalProteins),
                    Carbohydrates = logs.Sum(l => l.TotalCarbohydrates),
                    Fats = logs.Sum(l => l.TotalFats),
                    Fiber = logs.Sum(l => l.TotalFiber ?? 0),
                },
                DailyAverages = new NutritionAveragesDto
                {
                    Calories = logs.Average(l => l.TotalCalories),
                    Proteins = logs.Average(l => l.TotalProteins),
                    Carbohydrates = logs.Average(l => l.TotalCarbohydrates),
                    Fats = logs.Average(l => l.TotalFats)
                },
                DailyLogs = logs
            };

            return Result<DailyLogsRangeDto>.Ok(dto);
        }
    }
}