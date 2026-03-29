using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodEntryCommand;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.Services;
using Moq;
using System.Threading.Tasks;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;

namespace MealMind.Modules.Nutrition.Tests.Unit;

public class AddFoodEntryCommandTest
{
    private readonly Mock<IDailyLogRepository> _dailyLogRepositoryMock = new();
    private readonly Mock<IUserProfileRepository> _profileRepositoryMock = new();
    private readonly Mock<IFoodRepository> _foodRepositoryMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IOpenFoodFactsService> _factsServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Test]
    public async Task Handle_ValidRequest_ShouldAddFoodEntry()
    {
        var userId = UserId.New();

        var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
        var mealType = MealType.Lunch;
        var barcode = "1234567890123";
        var quantityInGrams = 100;
        var dailyLog = DailyLog.Create(dailyLogDate, null, 3000, userId);
        dailyLog.AddMeal(Meal.Initialize(mealType, userId));
        var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, barcode, null, quantityInGrams);

        _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));

        _dailyLogRepositoryMock.Setup(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);


        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(quantityInGrams).IsGreaterThanOrEqualTo(0);
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Message).IsNotEqualTo("Either Barcode or FoodId must be provided.");
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.Message).IsNotEqualTo("Meal not found for the specified meal type.");
    }
}