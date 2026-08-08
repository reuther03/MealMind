using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Application.Features.Caching;
using MealMind.Modules.Training.Application.Features.Mappers;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Pagination;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetAllTrainingPlansQuery(int Page, int PageSize) : IQuery<PaginatedList<TrainingPlanDto>>
{
    public sealed class Handler : IQueryHandler<GetAllTrainingPlansQuery, PaginatedList<TrainingPlanDto>>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;

        public Handler(ITrainingDbContext dbContext, IUserService userService, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _cacheService = cacheService;
        }

        public async Task<Result<PaginatedList<TrainingPlanDto>>> Handle(GetAllTrainingPlansQuery query,
            CancellationToken cancellationToken = default)
        {
            var userId = _userService.UserId!;
            var cacheKey = CacheKeyBuilder.GetAllTrainingPlansKey(userId);

            var trainingPlans = await _cacheService.GetAsync<List<TrainingPlanDto>>(cacheKey);
            if (trainingPlans is null)
            {
                trainingPlans = await _dbContext.TrainingPlans
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.Sessions.Where(s => s.EndedAt != null).Max(s => s.EndedAt))
                    .ThenBy(x => x.Name)
                    .Select(TrainingPlanMapper.Projection)
                    .ToListAsync(cancellationToken);

                await _cacheService.SetAsync(cacheKey, trainingPlans);
            }

            var paginatedTrainingPlans = trainingPlans
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Result<PaginatedList<TrainingPlanDto>>.Ok(new PaginatedList<TrainingPlanDto>(
                query.Page,
                query.PageSize,
                trainingPlans.Count,
                paginatedTrainingPlans));
        }
    }
}
