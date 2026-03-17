using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.InitializePlanCommand;

public record InitializePlanCommand : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<InitializePlanCommand, Guid>
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

        public async Task<Result<Guid>> Handle(InitializePlanCommand command, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<Guid>.BadRequest("User is not authenticated.");

            throw new NotImplementedException();
        }
    }
}