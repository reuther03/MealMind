using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Training.Application.Features.Commands.AddExerciseCommand;

public record AddExerciseCommand(Guid TrainingPlanId, Guid SessionId, Guid ExerciseId) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddExerciseCommand, Guid>
    {
        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            ITrainingPlanRepository trainingPlanRepository,
            IExerciseRepository exerciseRepository,
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _trainingPlanRepository = trainingPlanRepository;
            _exerciseRepository = exerciseRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddExerciseCommand command, CancellationToken cancellationToken)
        {
            var userId = _userService.UserId!;
            var trainingPlan = await _trainingPlanRepository.GetByIdAsync(command.TrainingPlanId, userId, cancellationToken);
            if (trainingPlan is null)
                return Result<Guid>.BadRequest("Training plan not found.");

            var session = trainingPlan.Sessions.FirstOrDefault(s => s.Id == command.SessionId);
            if (session is null)
                return Result<Guid>.BadRequest("Training session not found in the specified training plan.");

            var exercise = await _exerciseRepository.GetByIdAsync(command.ExerciseId, cancellationToken);
            if (exercise is null)
                return Result<Guid>.BadRequest("Exercise not found.");

            var sessionExercise = SessionExercise.CreateEmpty(exercise.Id, session.Exercises.Count + 1, exercise.Type);
            session.AddExercise(sessionExercise);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(sessionExercise.Id);
        }
    }
}