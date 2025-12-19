using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFavoriteFoodCommand;

public record AddFavoriteFoodCommand(Guid FoodId) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<AddFavoriteFoodCommand, bool>
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IFoodRepository foodRepository, IUserProfileRepository userProfileRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _foodRepository = foodRepository;
            _userProfileRepository = userProfileRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AddFavoriteFoodCommand command, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result.BadRequest<bool>("User profile not found.");

            var food = await _foodRepository.GetByIdAsync(command.FoodId, cancellationToken);
            if (food is null)
                return Result.BadRequest<bool>("Food not found.");

            user.AddFavoriteFood(food.Id);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(true);
        }
    }
}