using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.Payloads;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.UpdateNutritionTargetCommand;

public record UpdateNutritionTargetCommand(
    Guid NutritionTargetId,
    decimal Calories,
    NutritionInGramsPayload? NutritionInGramsPayload,
    NutritionInPercentPayload? NutritionInPercentPayload,
    decimal WaterIntake,
    List<DayOfWeek>? ActiveDays
) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<UpdateNutritionTargetCommand, bool>
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

        public async Task<Result<bool>> Handle(UpdateNutritionTargetCommand command, CancellationToken cancellationToken)
        {
            var nutritionTarget = await _userProfileRepository.GetNutritionTargetByIdAsync(command.NutritionTargetId, _userService.UserId, cancellationToken);
            if (nutritionTarget is null)
                return Result<bool>.NotFound("User profile not found.");

            if (command.NutritionInGramsPayload is null && command.NutritionInPercentPayload is null)
                return Result<bool>.BadRequest("Either Nutrition in grams or Nutrition in percent must be provided.");

            if (command.NutritionInGramsPayload is not null && command.NutritionInPercentPayload is not null)
                return Result<bool>.BadRequest("Only one of Nutrition in grams or Nutrition in percent can be provided.");

            if (command.NutritionInGramsPayload is not null)
            {
                nutritionTarget.UpdateForGrams(
                    command.Calories,
                    command.NutritionInGramsPayload.ProteinInGrams,
                    command.NutritionInGramsPayload.CarbohydratesInGrams,
                    command.NutritionInGramsPayload.FatsInGrams,
                    command.WaterIntake);
            }
            else
            {
                nutritionTarget.UpdateForPercentages(
                    command.Calories,
                    command.NutritionInPercentPayload!.ProteinPercentage,
                    command.NutritionInPercentPayload.CarbohydratesPercentage,
                    command.NutritionInPercentPayload.FatsPercentage,
                    command.WaterIntake);
            }

            nutritionTarget.UpdateActiveDays(command.ActiveDays ?? Enum.GetValues<DayOfWeek>().ToList());

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Ok(true);
        }
    }
}