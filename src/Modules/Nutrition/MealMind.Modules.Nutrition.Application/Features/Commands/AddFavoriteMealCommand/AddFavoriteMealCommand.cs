using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFavoriteMealCommand;

public record AddFavoriteMealCommand(Guid MealId) : ICommand<bool>
{
    public sealed class Handler : ICommandHandler<AddFavoriteMealCommand, bool>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IDailyLogRepository _dailyLogRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUserProfileRepository userProfileRepository, IDailyLogRepository dailyLogRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userProfileRepository = userProfileRepository;
            _dailyLogRepository = dailyLogRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AddFavoriteMealCommand command, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result.BadRequest<bool>("User profile not found.");

            var meal = await _dailyLogRepository.GetMealByIdAsync(command.MealId, cancellationToken);
            if (meal is null)
                return Result.BadRequest<bool>("Meal not found.");

            user.AddFavoriteMeal(meal.Id);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok(true);
        }
    }
}