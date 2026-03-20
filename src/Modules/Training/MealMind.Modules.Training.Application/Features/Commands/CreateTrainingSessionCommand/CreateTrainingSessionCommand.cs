using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.CreateTrainingSessionCommand;

public record CreateTrainingSessionCommand(Guid TrainingPlanId, string Name, string Description) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<CreateTrainingSessionCommand, Guid>
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


        public async Task<Result<Guid>> Handle(CreateTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<Guid>.BadRequest("User is not authenticated.");

            var trainingPlan = await _trainingPlanRepository.GetByIdAsync(command.TrainingPlanId, _userService.UserId, cancellationToken);
            if (trainingPlan is null)
                return Result<Guid>.NotFound("Training plan not found.");

            var trainingSession = TrainingSession.Create(command.Name, command.Description);

            trainingPlan.AddSession(trainingSession);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(trainingSession.Id);
        }
    }
}