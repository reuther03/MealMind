using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.CreatePlanCommand;

public record CreatePlanCommand(string Name, DayOfWeek PlannedAt) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<CreatePlanCommand, Guid>
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

        public async Task<Result<Guid>> Handle(CreatePlanCommand command, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<Guid>.BadRequest("User is not authenticated.");

            var trainingPlan = TrainingPlan.Create(command.Name, command.PlannedAt, _userService.UserId);

            await _trainingPlanRepository.AddAsync(trainingPlan, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(trainingPlan.Id.Value);
        }
    }
}