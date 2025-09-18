using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddNutritionTargetCommand;

public record AddNutritionTargetCommand(
    decimal Calories,
    NutritionInGramsPayload? NutritionInGramsPayload,
    NutritionInPercentPayload? NutritionInPercentPayload,
    decimal WaterIntake,
    List<DayOfWeek> ActiveDays
) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddNutritionTargetCommand, Guid>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUserProfileRepository userProfileRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userProfileRepository = userProfileRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddNutritionTargetCommand command, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileRepository.GetWithIncludesByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(userProfile);

            if (command.NutritionInGramsPayload is null && command.NutritionInPercentPayload is null)
                return Result<Guid>.BadRequest("Either Nutrition in grams or Nutrition in percent must be provided.");

            if (command.NutritionInGramsPayload is not null && command.NutritionInPercentPayload is not null)
                return Result<Guid>.BadRequest("Only one of Nutrition in grams or Nutrition in percent can be provided.");

            var nutritionTarget = command.NutritionInGramsPayload is not null
                ? NutritionTarget.CreateFromGrams(
                    command.Calories,
                    command.NutritionInGramsPayload.ProteinInGrams,
                    command.NutritionInGramsPayload.CarbohydratesInGrams,
                    command.NutritionInGramsPayload.FatsInGrams,
                    command.WaterIntake,
                    userProfile.Id)
                : NutritionTarget.CreateFromPercentages(
                    command.Calories,
                    command.NutritionInPercentPayload!.ProteinPercentage,
                    command.NutritionInPercentPayload.CarbohydratesPercentage,
                    command.NutritionInPercentPayload.FatsPercentage,
                    command.WaterIntake,
                    userProfile.Id);

            nutritionTarget.AddActiveDay(command.ActiveDays);

            userProfile.AddNutritionTarget(nutritionTarget);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(nutritionTarget.Id);
        }
    }
}