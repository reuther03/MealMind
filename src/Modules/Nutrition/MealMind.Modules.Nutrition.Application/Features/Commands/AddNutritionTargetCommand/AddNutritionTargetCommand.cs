using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;

namespace MealMind.Modules.Nutrition.Application.Features.Commands;

public record AddNutritionTargetCommand(int Calories, int ProteinPercentage, int CarbohydratesPercentage, int FatsPercentage, int WaterIntake) : ICommand<Guid>
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
            var userProfile = await _userProfileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(userProfile);

            if (command.FatsPercentage + command.ProteinPercentage + command.CarbohydratesPercentage != 100)
                return Result<Guid>.BadRequest("The sum of fats, protein, and carbohydrates percentages must equal 100.");

            var protein = (int)Math.Round(command.Calories * (command.ProteinPercentage / 4.0) / 100);
            var carbohydrates = (int)Math.Round(command.Calories * (command.CarbohydratesPercentage / 4.0) / 100);
            var fats = (int)Math.Round(command.Calories * (command.FatsPercentage / 9.0) / 100);

            var nutritionTarget =
                NutritionTarget.Create(command.Calories, protein, carbohydrates, fats, command.WaterIntake, _userService.UserId);
        }
    }
}