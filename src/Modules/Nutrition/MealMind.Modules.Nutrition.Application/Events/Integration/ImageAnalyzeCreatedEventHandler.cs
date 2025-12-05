using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;
using MealMind.Shared.Events.AiChat;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public class ImageAnalyzeCreatedEventHandler : IEventHandler<ImageAnalyzeCreatedEvent>
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImageAnalyzeCreatedEventHandler> _logger;

    public ImageAnalyzeCreatedEventHandler(IDailyLogRepository dailyLogRepository, IUnitOfWork unitOfWork, ILogger<ImageAnalyzeCreatedEventHandler> logger)
    {
        _dailyLogRepository = dailyLogRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ImageAnalyzeCreatedEvent notification, CancellationToken cancellationToken)
    {
        var dailyLog = await _dailyLogRepository.GetByDateAsync(notification.DailyLogDate, notification.UserId, cancellationToken);
        if (dailyLog == null)
        {
            _logger.LogWarning("Daily log for user {UserId} on date {Date} not found. Cannot add food entry from image analyze.", notification.UserId,
                notification.DailyLogDate);
            return;
        }

        var foodEntry = FoodEntry.CreateFromImageAnalyze(
            notification.FoodName,
            notification.QuantityInGrams,
            notification.TotalCalories,
            notification.TotalProteins,
            notification.TotalCarbohydrates,
            notification.TotalFats
        );

        //todo: currently adding to the first meal, later we can improve this by allowing users to specify meal type
        var meal = dailyLog.Meals[0];
        meal.AddFood(foodEntry);

        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Added food entry from image analyze to daily log for user {UserId} on date {Date}. Food: {FoodName}, Quantity: {Quantity}g",
            notification.UserId, notification.DailyLogDate, notification.FoodName, notification.QuantityInGrams);
    }
}