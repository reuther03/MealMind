using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public record IdentityUserCreatedEventHandler : INotificationHandler<IdentityUserCreatedEvent>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IdentityUserCreatedEventHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(IdentityUserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var userProfile = UserProfile.Create(notification.Id, notification.Username, notification.Email, SubscriptionTier.Free);

        await _userProfileRepository.AddAsync(userProfile, cancellationToken);

        var personalData = PersonalData.Create(
            notification.PersonalData.Gender,
            notification.PersonalData.DateOfBirth,
            notification.PersonalData.Weight,
            notification.PersonalData.Height,
            notification.PersonalData.WeightTarget,
            notification.PersonalData.ActivityLevel
        );

        userProfile.SetPersonalData(personalData);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}