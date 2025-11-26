using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public class ImageAnalyzeCreatedEventHandler : INotificationHandler<ImageAnalyzeCreatedEvent>
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IUnitOfWork _unitOfWork;


    public Task Handle(ImageAnalyzeCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}