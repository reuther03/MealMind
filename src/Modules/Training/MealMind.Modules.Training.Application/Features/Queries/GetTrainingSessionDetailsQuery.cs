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

public record GetTrainingSessionDetailsQuery(Guid PlanId, Guid TrainingSessionId) : IQuery<TrainingSessionDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingSessionDetailsQuery, TrainingSessionDetailsDto>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(ITrainingDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<TrainingSessionDetailsDto>> Handle(GetTrainingSessionDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _userService.UserId;

            var session = await _dbContext.TrainingPlans
                .AsExpandable()
                .Where(p => p.Id == TrainingPlanId.From(request.PlanId) && p.UserId == userId && p.IsActive)
                .SelectMany(p => p.Sessions)
                .Where(s => s.Id == request.TrainingSessionId)
                .Select(s => TrainingSessionMapper.DetailsProjection.Invoke(s))
                .FirstOrDefaultAsync(cancellationToken);

            return session == null
                ? Result<TrainingSessionDetailsDto>.NotFound("Training session not found.")
                : Result<TrainingSessionDetailsDto>.Ok(session);
        }
    }
}
