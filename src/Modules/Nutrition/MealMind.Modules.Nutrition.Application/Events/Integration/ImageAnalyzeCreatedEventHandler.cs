using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public class ImageAnalyzeCreatedEventHandler : INotificationHandler<ImageAnalyzeCreatedEvent>
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImageAnalyzeCreatedEventHandler(IDailyLogRepository dailyLogRepository, IUnitOfWork unitOfWork)
    {
        _dailyLogRepository = dailyLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ImageAnalyzeCreatedEvent notification, CancellationToken cancellationToken)
    {
        var dailyLog = await _dailyLogRepository.GetByDateAsync(notification.DailyLogDate, notification.UserId, cancellationToken);
        if (dailyLog == null)
            throw new ApplicationException($"Daily log for date {notification.DailyLogDate} not found.");

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
    }
}