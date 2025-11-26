using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Events.Integration;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.QueriesAndCommands.Notifications;

namespace MealMind.Modules.Nutrition.Application.Events.Integration;

public record IdentityUserCreatedEventHandler : INotificationHandler<IdentityUserCreatedEvent>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IdentityUserCreatedEventHandler(IUserProfileRepository userProfileRepository, IDailyLogRepository dailyLogRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _dailyLogRepository = dailyLogRepository;
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

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        List<DailyLog> dailyLogs = [];
        for (var i = 0; i < 90; i++)
        {
            var logDate = today.AddDays(i);
            var dailyLog = DailyLog.Create(
                logDate,
                null,
                userProfile.NutritionTargets
                    .FirstOrDefault(x => x.ActiveDays
                        .Any(z => z.DayOfWeek == logDate.DayOfWeek))?.Calories ?? 2000,
                userProfile.Id
            );

            foreach (var type in Enum.GetValues<MealType>())
            {
                var meal = Meal.Initialize(type, userProfile.Id);
                dailyLog.AddMeal(meal);
            }

            dailyLogs.Add(dailyLog);
        }

        await _dailyLogRepository.AddRangeAsync(dailyLogs, cancellationToken);
        userProfile.SetPersonalData(personalData);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}