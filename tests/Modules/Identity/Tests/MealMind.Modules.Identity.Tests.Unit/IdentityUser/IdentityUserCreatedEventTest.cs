using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Types;
using MealMind.Shared.Events.Identity;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.IdentityUser;

public class IdentityUserCreatedEventTest
{
    private readonly Mock<IUserProfileRepository> _userProfileRepository;
    private readonly Mock<IDailyLogRepository> _dailyLogRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;


    [Test]
    public async Task IdentityUserCreated_ShouldCoverAllDaysProvided()
    {
        var nutritionTargets = new List<NutritionTargetPayload>
        {
            new NutritionTargetPayload(2000, null, null, 2, new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday }),
            new NutritionTargetPayload(2200, null, null, 2.5m, new List<DayOfWeek> { DayOfWeek.Thursday, DayOfWeek.Friday }),
            new NutritionTargetPayload(2500, null, null, 3, new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday })
        };

        var personalData = new PersonalDataPayload(Gender.Male, DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), 180, 80, 75, ActivityLevel.Active);

        var eventPayload = new IdentityUserCreatedEvent(
            Guid.NewGuid(),
            "testUser",
            "test@test.com",
            personalData,
            nutritionTargets
        );
    }
}