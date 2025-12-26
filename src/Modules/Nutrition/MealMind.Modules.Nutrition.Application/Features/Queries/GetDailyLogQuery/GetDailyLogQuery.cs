using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Dtos.DailyLogDtos;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
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
                .Include(x => x.Meals
                    .OrderBy(z => (int)z.MealType))
                .ThenInclude(x => x.Foods)
                .Select(x => DailyLogDto.AsDto(x))
                .SingleOrDefaultAsync(cancellationToken);

            return dailyLogDto is null
                ? Result<DailyLogDto>.BadRequest("Daily log not found for the specified date.")
                : Result<DailyLogDto>.Ok(dailyLogDto);
        }
    }
}