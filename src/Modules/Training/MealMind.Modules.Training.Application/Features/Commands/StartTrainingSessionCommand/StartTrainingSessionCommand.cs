using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.StartTrainingSessionCommand;

public record StartTrainingSessionCommand(Guid TrainingPlanId, Guid TrainingSessionId) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<StartTrainingSessionCommand, bool>
    {
        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ITrainingPlanRepository trainingPlanRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _trainingPlanRepository = trainingPlanRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(StartTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<bool>.BadRequest("User is not authenticated.");

            var userId = _userService.UserId;

            var trainingPlan = await _trainingPlanRepository.GetByIdAsync(command.TrainingPlanId, userId, cancellationToken);
            if (trainingPlan is null)
                return Result<bool>.NotFound("Training plan not found.");

            var trainingSession = trainingPlan.Sessions.FirstOrDefault(x => x.Id == command.TrainingSessionId);
            if (trainingSession is null)
                return Result<bool>.NotFound("Training session not found.");


            trainingSession.SetAsStarted();

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<bool>.Ok(true);
        }
    }
}