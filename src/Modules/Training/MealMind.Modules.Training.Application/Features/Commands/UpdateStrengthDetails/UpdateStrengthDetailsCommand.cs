using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.UpdateStrengthDetails;

public record UpdateStrengthDetailsCommand(Guid PlanId, Guid SessionId, Guid ExerciseId, StrengthDetails StrengthDetails) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<UpdateStrengthDetailsCommand, bool>
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

        public async Task<Result<bool>> Handle(UpdateStrengthDetailsCommand command, CancellationToken cancellationToken)
        {
            var trainingPlan = await _trainingPlanRepository.GetByIdAsync(command.PlanId, _userService.UserId!, cancellationToken);
            if (trainingPlan is null)
                return Result<bool>.NotFound("Training plan not found.");

            var session = trainingPlan.Sessions.FirstOrDefault(s => s.Id == command.SessionId);
            if (session is null)
                return Result<bool>.NotFound("Session not found.");

            var exercise = session.Exercises.FirstOrDefault(e => e.Id == command.ExerciseId);
            if (exercise is null)
                return Result<bool>.NotFound("Exercise not found in session.");

            exercise.SetStrengthDetails(command.StrengthDetails);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Ok(true);
        }
    }
}