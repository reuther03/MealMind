using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodCommand;

public record AddFoodCommand(DateOnly DailyLogDate, MealType MealType) : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddFoodCommand, Guid>
    {
        private readonly IDailyLogRepository _dailyLogRepository;
        private readonly IUserProfileRepository _profileRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IDailyLogRepository dailyLogRepository, IUserProfileRepository profileRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _dailyLogRepository = dailyLogRepository;
            _profileRepository = profileRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddFoodCommand request, CancellationToken cancellationToken)
        {
            var user = await _profileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(user);

            var dailyLog = await _dailyLogRepository.GetByDateAsync(request.DailyLogDate, _userService.UserId, cancellationToken);
            if (dailyLog is null) //or check ExistsWithDate and if else
            {
                dailyLog = DailyLog.Create(
                    user.GetWeightHistory(request.DailyLogDate).Weight,
                    user.NutritionTargets
                        .Where(x => x.ActiveDays
                            .Any(z => z.DayOfWeek == request.DailyLogDate.DayOfWeek))
                        .Select(x => x.Calories)
                        .FirstOrDefault(),
                    // use this above or domain method to get calories for current date based on active nutrition targets?
                    user.Id);

                foreach (var type in Enum.GetValues<MealType>())
                {
                    var meal = Meal.Initialize(type, user.Id);
                    dailyLog.AddMeal(meal);
                }

                await _dailyLogRepository.AddAsync(dailyLog, cancellationToken);
            }


            throw new NotImplementedException();
        }
    }
}