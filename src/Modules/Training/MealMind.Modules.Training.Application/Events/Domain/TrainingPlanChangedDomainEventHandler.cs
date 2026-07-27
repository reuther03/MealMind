using MealMind.Modules.Training.Application.Features.Caching;
using MealMind.Modules.Training.Domain.Events;
using MealMind.Shared.Abstractions.Kernel.Events;
using MealMind.Shared.Abstractions.Services;

namespace MealMind.Modules.Training.Application.Events.Domain;

public record TrainingPlanChangedDomainEventHandler : IDomainNotificationHandler<TrainingPlanChangedDomainEvent>
{
    private readonly ICacheService _cacheService;

    public TrainingPlanChangedDomainEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(TrainingPlanChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var test = 0;
        await _cacheService.RemoveAsync(
            CacheKeyBuilder.GetTrainingPlanDetailsKey(notification.UserId, notification.TrainingPlanId));
    }
}
