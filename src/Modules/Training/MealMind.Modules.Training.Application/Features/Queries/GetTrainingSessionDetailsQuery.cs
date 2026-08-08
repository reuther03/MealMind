using LinqKit;
using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Application.Features.Caching;
using MealMind.Modules.Training.Application.Features.Mappers;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetTrainingSessionDetailsQuery(Guid PlanId, Guid TrainingSessionId) : IQuery<TrainingSessionDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingSessionDetailsQuery, TrainingSessionDetailsDto>
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

        public async Task<Result<TrainingSessionDetailsDto>> Handle(GetTrainingSessionDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _userService.UserId!;

            if (await _cacheService.GetAsync<TrainingSessionDetailsDto>(
                    CacheKeyBuilder.GetTrainingSessionDetailsKey(userId, request.PlanId, request.TrainingSessionId)) is
                { } cachedSession)
            {
                return Result<TrainingSessionDetailsDto>.Ok(cachedSession);
            }

            var session = await _dbContext.TrainingPlans
                .AsExpandable()
                .Where(p => p.Id == TrainingPlanId.From(request.PlanId) && p.UserId == userId && p.IsActive)
                .SelectMany(p => p.Sessions)
                .Where(s => s.Id == request.TrainingSessionId)
                .Select(s => TrainingSessionMapper.DetailsProjection.Invoke(s))
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return Result<TrainingSessionDetailsDto>.NotFound("Training session not found.");

            await _cacheService.SetAsync(CacheKeyBuilder.GetTrainingSessionDetailsKey(userId, request.PlanId, request.TrainingSessionId), session);
            return Result<TrainingSessionDetailsDto>.Ok(session);
        }
    }
}
