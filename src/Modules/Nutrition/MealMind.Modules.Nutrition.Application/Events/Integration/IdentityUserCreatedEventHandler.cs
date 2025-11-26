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

        var allDaysOfWeek = Enum.GetValues<DayOfWeek>().ToHashSet();
        var coveredDays = new HashSet<DayOfWeek>();
        foreach (var target in notification.NutritionTargets)
        {
            if (target.ActiveDays == null)
            {
                coveredDays = allDaysOfWeek;
                break;
            }

            coveredDays.UnionWith(target.ActiveDays);
        }

        if (!coveredDays.SetEquals(allDaysOfWeek))
            throw new ApplicationException("Active days must cover all days of the week");

        foreach (var targetPayload in notification.NutritionTargets)
        {
            if (targetPayload.NutritionInGramsPayload is null && targetPayload.NutritionInPercentPayload is null)
                throw new InvalidOperationException("Either Nutrition in grams or Nutrition in percent must be provided.");

            var nutritionTarget = targetPayload.NutritionInGramsPayload is not null
                ? NutritionTarget.CreateFromGrams(
                    targetPayload.Calories,
                    targetPayload.NutritionInGramsPayload.ProteinInGrams,
                    targetPayload.NutritionInGramsPayload.CarbohydratesInGrams,
                    targetPayload.NutritionInGramsPayload.FatsInGrams,
                    targetPayload.WaterIntake,
                    userProfile.Id)
                : NutritionTarget.CreateFromPercentages(
                    targetPayload.Calories,
                    targetPayload.NutritionInPercentPayload!.ProteinPercentage,
                    targetPayload.NutritionInPercentPayload.CarbohydratesPercentage,
                    targetPayload.NutritionInPercentPayload.FatsPercentage,
                    targetPayload.WaterIntake,
                    userProfile.Id);

            nutritionTarget.AddActiveDay(targetPayload.ActiveDays ?? Enum.GetValues<DayOfWeek>().ToList());
            userProfile.AddNutritionTarget(nutritionTarget);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        List<DailyLog> dailyLogs = [];
        for (var i = 0; i < 90; i++)
        {
            var logDate = today.AddDays(i);
            var calorieTarget = userProfile.NutritionTargets
                .FirstOrDefault(x => x.ActiveDays
                    .Any(z => z.DayOfWeek == logDate.DayOfWeek));

            if (calorieTarget == null)
                throw new InvalidOperationException(
                    $"No nutrition target found for {logDate.DayOfWeek}. This indicates a validation error.");

            var dailyLog = DailyLog.Create(logDate, null, calorieTarget.Calories, userProfile.Id);

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