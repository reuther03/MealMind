using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Events.Core;
using MealMind.Shared.Events.Identity;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public class NutritionSubscriptionTierUpdatedEventHandler : IEventHandler<SubscriptionTierUpdatedEvent>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NutritionSubscriptionTierUpdatedEventHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubscriptionTierUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileRepository.GetByIdAsync(notification.UserId, cancellationToken);
        if (userProfile is null)
            return;

        userProfile.UpdateSubscriptionTier(notification.SubscriptionTier);
        _userProfileRepository.Update(userProfile);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}