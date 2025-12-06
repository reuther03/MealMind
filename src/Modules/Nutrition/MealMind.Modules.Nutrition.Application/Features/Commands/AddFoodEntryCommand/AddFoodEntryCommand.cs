using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Application.Dtos;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.QueriesAndCommands.Commands;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;

namespace MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodEntryCommand;

public record AddFoodEntryCommand(DateOnly DailyLogDate, MealType MealType, string? Barcode, Guid? FoodId, decimal QuantityInGrams)
    : ICommand<Guid>
{
    public sealed class Handler : ICommandHandler<AddFoodEntryCommand, Guid>
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

        public async Task<Result<Guid>> Handle(AddFoodEntryCommand request, CancellationToken cancellationToken)
        {
            var user = await _profileRepository.GetWithIncludesByIdAsync(_userService.UserId, cancellationToken);
            if (user is null)
                return Result<Guid>.NotFound("User profile not found.");

            if (request.Barcode is null && request.FoodId is null)
                return Result<Guid>.BadRequest("Either Barcode or FoodId must be provided.");

            var dailyLog = await _dailyLogRepository.GetByDateAsync(request.DailyLogDate, user.Id, cancellationToken);
            if (dailyLog is null)
                return Result<Guid>.NotFound("Daily log not found for the specified date.");

            var requestMeal = dailyLog.Meals.FirstOrDefault(x => x.MealType == request.MealType);
            if (requestMeal is null)
                return Result<Guid>.NotFound("Meal not found for the specified meal type.");

            if (request.QuantityInGrams <= 0)
                return Result<Guid>.BadRequest("Quantity must be greater than zero.");

            Food? food = null;
            if (request.FoodId is not null)
            {
                food = await _foodRepository.GetByIdAsync(request.FoodId.Value, cancellationToken);
            }
            else if (request.Barcode is not null)
            {
                food = await _foodRepository.GetByBarcodeAsync(request.Barcode, cancellationToken);
                if (food is null)
                {
                    var foodDtoResult = await _factsService.GetFoodByBarcodeAsync(request.Barcode, cancellationToken);
                    if (foodDtoResult is null)
                        return Result<Guid>.NotFound("Food not found by the provided barcode.");

                    food = FoodDto.ToEntity(foodDtoResult);
                    var stats = FoodStatistics.Initialize(food.Id);
                    food.UpdateStatistics(stats);

                    await _foodRepository.AddAsync(food, cancellationToken);
                }
            }

            if (food is null)
                return Result<Guid>.NotFound("Food not found.");


            var foodEntry = FoodEntry.Create(food, request.QuantityInGrams);
            requestMeal.AddFood(foodEntry);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(foodEntry.Id);
        }
    }
}