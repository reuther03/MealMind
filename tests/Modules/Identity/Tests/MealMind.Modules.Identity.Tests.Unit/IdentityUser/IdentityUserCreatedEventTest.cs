using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Events.Integration;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Modules.Nutrition.Domain.UserProfile;
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

    public IdentityUserCreatedEventTest()
    {
        _userProfileRepository = new Mock<IUserProfileRepository>();
        _dailyLogRepository = new Mock<IDailyLogRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
    }


    [Test]
    public async Task Handle_AllDaysCoveredExplicitly_ShouldCreateProfileAndLogs()
    {
        var nutritionTargets = new List<NutritionTargetPayload>
        {
            new(2000, null, new NutritionInPercentPayload(30, 30, 40), 2, [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]),
            new(2200, null, new NutritionInPercentPayload(30, 30, 40), 2.5m, [DayOfWeek.Thursday, DayOfWeek.Friday]),
            new(2500, null, new NutritionInPercentPayload(30, 30, 40), 3, [DayOfWeek.Saturday, DayOfWeek.Sunday])
        };

        var personalData = new PersonalDataPayload(Gender.Male, DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), 180, 80, 75, ActivityLevel.Active);
        var eventPayload = new IdentityUserCreatedEvent(
            Guid.NewGuid(),
            "testUser",
            "test@test.com",
            personalData,
            nutritionTargets
        );

        var eventHandler = new IdentityUserCreatedEventHandler(_userProfileRepository.Object, _dailyLogRepository.Object, _unitOfWork.Object);
        await eventHandler.Handle(eventPayload, CancellationToken.None);

        _userProfileRepository.Verify(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepository.Verify(x => x.AddRangeAsync(It.Is<List<DailyLog>>(l => l.Count == 90), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_NullActiveDays_ShouldCoverAllDaysAutomatically()
    {
        var nutritionTargets = new List<NutritionTargetPayload>
        {
            new(2000, null, new NutritionInPercentPayload(30, 30, 40), 2, null),
        };

        var personalData = new PersonalDataPayload(Gender.Male, DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), 180, 80, 75, ActivityLevel.Active);
        var eventPayload = new IdentityUserCreatedEvent(
            Guid.NewGuid(),
            "testUser",
            "test@test.com",
            personalData,
            nutritionTargets
        );

        var eventHandler = new IdentityUserCreatedEventHandler(_userProfileRepository.Object, _dailyLogRepository.Object, _unitOfWork.Object);
        await eventHandler.Handle(eventPayload, CancellationToken.None);

        _userProfileRepository.Verify(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepository.Verify(x => x.AddRangeAsync(It.Is<List<DailyLog>>(l => l.Count == 90), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_IncompleteDaysCoverage_ShouldThrow()
    {
        var nutritionTargets = new List<NutritionTargetPayload>
        {
            new(2000, null, new NutritionInPercentPayload(30, 30, 40), 2, [DayOfWeek.Monday, DayOfWeek.Wednesday]),
            new(2200, null, new NutritionInPercentPayload(30, 30, 40), 2.5m, [DayOfWeek.Thursday, DayOfWeek.Friday]),
            new(2500, null, new NutritionInPercentPayload(30, 30, 40), 3, [DayOfWeek.Saturday, DayOfWeek.Sunday])
        };

        var personalData = new PersonalDataPayload(Gender.Male, DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), 180, 80, 75, ActivityLevel.Active);
        var eventPayload = new IdentityUserCreatedEvent(
            Guid.NewGuid(),
            "testUser",
            "test@test.com",
            personalData,
            nutritionTargets
        );

        var eventHandler = new IdentityUserCreatedEventHandler(_userProfileRepository.Object, _dailyLogRepository.Object, _unitOfWork.Object);
        await Assert.ThrowsAsync<ApplicationException>(()
            => eventHandler.Handle(eventPayload, CancellationToken.None));

        _userProfileRepository.Verify(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        _dailyLogRepository.Verify(x => x.AddRangeAsync(It.Is<List<DailyLog>>(l => l.Count == 90), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}