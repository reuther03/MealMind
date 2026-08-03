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

public record GetTrainingPlanDetailsQuery(Guid PlanId) : IQuery<TrainingPlanDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingPlanDetailsQuery, TrainingPlanDetailsDto>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IUserService _userService;

        public Handler(ITrainingDbContext dbContext, ICacheService cacheService, IUserService userService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _userService = userService;
        }

        public async Task<Result<TrainingPlanDetailsDto>> Handle(GetTrainingPlanDetailsQuery request,
            CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<TrainingPlanDetailsDto>.BadRequest("User is not authenticated.");

            var userId = _userService.UserId;

            if (await _cacheService.GetAsync<TrainingPlanDetailsDto>(
                    CacheKeyBuilder.GetTrainingPlanDetailsKey(userId, request.PlanId)) is
                { } cachedTrainingPlan)
            {
                return Result<TrainingPlanDetailsDto>.Ok(cachedTrainingPlan);
            }

            var trainingPlan = await _dbContext.TrainingPlans
                .AsExpandable()
                .Where(x => x.Id == TrainingPlanId.From(request.PlanId) && x.UserId == userId && x.IsActive)
                .Select(TrainingPlanMapper.DetailsProjection)
                .FirstOrDefaultAsync(cancellationToken);

            if (trainingPlan is null)
                return Result<TrainingPlanDetailsDto>.NotFound("No training plan was found.");

            await _cacheService.SetAsync(
                CacheKeyBuilder.GetTrainingPlanDetailsKey(userId, request.PlanId),
                trainingPlan);

            return Result<TrainingPlanDetailsDto>.Ok(trainingPlan);
        }
    }
}
