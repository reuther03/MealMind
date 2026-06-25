using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.EndTrainingSessionCommand;

public record EndTrainingSessionCommand(Guid PlanId, Guid SessionId) : ICommand<SessionComparisonDto>
{
    public sealed class Handler : ICommandHandler<EndTrainingSessionCommand, SessionComparisonDto>
    {
        private readonly IUserService _userService;
        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ISessionComparisonService _sessionComparisonService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUserService userService, ITrainingPlanRepository trainingPlanRepository, ISessionComparisonService sessionComparisonService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _trainingPlanRepository = trainingPlanRepository;
            _sessionComparisonService = sessionComparisonService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SessionComparisonDto>> Handle(EndTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            var userId = _userService.UserId;

            var trainingPlan = await _trainingPlanRepository.GetByIdAsync(command.PlanId, userId!, cancellationToken);
            if (trainingPlan is null)
                return Result<SessionComparisonDto>.NotFound("Training plan not found.");

            var sessionToEnd = trainingPlan.Sessions.FirstOrDefault(x => x.Id == command.SessionId && !x.IsCompleted);
            if (sessionToEnd is null)
                return Result<SessionComparisonDto>.NotFound("Completed training session not found");

            if (!sessionToEnd.IsStarted)
                return Result<SessionComparisonDto>.BadRequest("Training session has not been started yet.");

            sessionToEnd.SetAsEnded();

            await _unitOfWork.CommitAsync(cancellationToken);

            var sessionComparison = await _sessionComparisonService.CompareSessionsAsync(command.SessionId, cancellationToken);

            return Result<SessionComparisonDto>.Ok(sessionComparison);
        }
    }
}