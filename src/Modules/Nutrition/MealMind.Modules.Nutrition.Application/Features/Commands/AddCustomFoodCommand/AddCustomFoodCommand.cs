using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddCustomFoodCommand;

public record AddCustomFoodCommand(
    string Name,
    string? Barcode,
    NutritionPer100G NutritionPer100G,
    string ImageUrl,
    string Brand,
    List<FoodCategory> Categories,
    List<FoodDietaryTag> DietaryTags
) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddCustomFoodCommand, Guid>
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IUserService _userService;
        private readonly IUserProfileRepository _profileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IFoodRepository foodRepository, IUserService userService, IUserProfileRepository profileRepository, IUnitOfWork unitOfWork)
        {
            _foodRepository = foodRepository;
            _userService = userService;
            _profileRepository = profileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddCustomFoodCommand command, CancellationToken cancellationToken)
        {
            var user = await _profileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result<Guid>.BadRequest("User profile not found.");

            var food = Food.Create(
                command.Name, command.NutritionPer100G,
                FoodDataSource.User, command.Barcode,
                command.ImageUrl, command.Brand,
                command.Categories, command.DietaryTags);

            await _foodRepository.AddAsync(food, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(food.Id.Value);
        }
    }
}