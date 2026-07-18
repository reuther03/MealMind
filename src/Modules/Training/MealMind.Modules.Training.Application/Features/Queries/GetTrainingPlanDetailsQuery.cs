using LinqKit;
using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Application.Features.Mappers;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetTrainingPlanDetailsQuery(Guid Id) : IQuery<TrainingPlanDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingPlanDetailsQuery, TrainingPlanDetailsDto>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(ITrainingDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<TrainingPlanDetailsDto>> Handle(GetTrainingPlanDetailsQuery request,
            CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<TrainingPlanDetailsDto>.BadRequest("User is not authenticated.");

            var userId = _userService.UserId;

            var trainingPlan = await _dbContext.TrainingPlans
                .AsExpandable()
                .Where(x => x.Id == TrainingPlanId.From(request.Id) && x.UserId == userId && x.IsActive)
                .Select(TrainingPlanMapper.DetailsProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return trainingPlan == null
                ? Result<TrainingPlanDetailsDto>.NotFound("Training plan not found.")
                : Result<TrainingPlanDetailsDto>.Ok(trainingPlan);
        }
    }
}
