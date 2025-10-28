using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Application.Dtos;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.CommandValidators;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodCommand;

public record AddFoodCommand(DateOnly DailyLogDate, MealType MealType, string? Barcode, Guid? FoodId, decimal QuantityInGrams, decimal CurrentWeight)
    : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddFoodCommand, Guid>
    {
        private readonly IDailyLogRepository _dailyLogRepository;
        private readonly IUserProfileRepository _profileRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IUserService _userService;
        private readonly IOpenFoodFactsService _factsService;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IDailyLogRepository dailyLogRepository, IUserProfileRepository profileRepository, IFoodRepository foodRepository,
            IUserService userService,
            IOpenFoodFactsService factsService, IUnitOfWork unitOfWork)
        {
            _dailyLogRepository = dailyLogRepository;
            _profileRepository = profileRepository;
            _foodRepository = foodRepository;
            _userService = userService;
            _factsService = factsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddFoodCommand request, CancellationToken cancellationToken)
        {
            var user = await _profileRepository.GetByIdAsync(_userService.UserId, cancellationToken);
            NullValidator.ValidateNotNull(user);

            if (request.Barcode is null && request.FoodId is null)
                return Result<Guid>.BadRequest("Either Barcode or FoodId must be provided.");

            DailyLog dailyLog;
            if (!await _dailyLogRepository.ExistsWithDateAsync(request.DailyLogDate, user.Id, cancellationToken)) //or check ExistsWithDate and if else
            {
                dailyLog = DailyLog.Create(
                    request.CurrentWeight,
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
            else
            {
                // without ! there is a warning even if we check ExistsWithDateAsync
                dailyLog = (await _dailyLogRepository.GetByDateAsync(request.DailyLogDate, _userService.UserId, cancellationToken))!;
            }

            var requestMeal = dailyLog.Meals.FirstOrDefault(x => x.MealType == request.MealType);
            NullValidator.ValidateNotNull(requestMeal);

            var food = request.FoodId is not null
                ? await _foodRepository.GetByIdAsync(request.FoodId.Value, cancellationToken)
                : FoodDto.ToEntity(await _factsService.GetFoodByBarcodeAsync(request.Barcode!, cancellationToken));

            NullValidator.ValidateNotNull(food);

            var foodEntry = FoodEntry.Create(food, request.QuantityInGrams);

            requestMeal.AddFood(foodEntry);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(foodEntry.Id);
        }
    }
}