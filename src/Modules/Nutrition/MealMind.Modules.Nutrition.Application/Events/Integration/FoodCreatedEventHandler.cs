using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Events.AiChat;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public class FoodCreatedEventHandler : IEventHandler<FoodCreatedEvent>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FoodCreatedEventHandler> _logger;

    public FoodCreatedEventHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork, ILogger<FoodCreatedEventHandler> logger)
    {
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(FoodCreatedEvent notification, CancellationToken cancellationToken)
    {
        var nutritionPer100G = new NutritionPer100G(
            notification.Food.NutritionPer100G.Calories,
            notification.Food.NutritionPer100G.Protein,
            notification.Food.NutritionPer100G.Carbohydrates,
            notification.Food.NutritionPer100G.Fat,
            notification.Food.NutritionPer100G.Salt,
            notification.Food.NutritionPer100G.Sugar,
            notification.Food.NutritionPer100G.SaturatedFat,
            notification.Food.NutritionPer100G.Fiber,
            notification.Food.NutritionPer100G.Sodium,
            notification.Food.NutritionPer100G.Cholesterol
        );

        var food = Food.Create(
            notification.Food.Name,
            nutritionPer100G,
            notification.Food.FoodSource,
            notification.Food.Barcode,
            notification.Food.ImageUrl,
            notification.Food.Brand
        );

        await _foodRepository.AddAsync(food, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Food created: {FoodName} (ID: {FoodId})", food.Name, food.Id);
    }
}