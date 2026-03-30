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
using MealMind.Shared.Contracts.Dto.Nutrition;

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
    public async Task Handle_ValidRequestWithBarcode_ShouldAddFoodEntry()
    {
        var userId = UserId.New();
        var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
        var mealType = MealType.Lunch;
        var barcode = "1234567890123";
        var quantityInGrams = 100;

        var dailyLog = DailyLog.Create(dailyLogDate, null, 3000, userId);
        dailyLog.AddMeal(Meal.Initialize(mealType, userId));

        var food = Food.Create("Test Food", new NutritionPer100G(330, 10, 30, 5, 8, 2, 0), FoodDataSource.Database, barcode);

        var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, barcode, null, quantityInGrams);

        _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));

        _dailyLogRepositoryMock.Setup(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);

        _foodRepositoryMock.Setup(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(food);


        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.IsSuccess).IsTrue();
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ValidRequestWithBarcodeFallbackPath_ShouldAddFoodEntry()
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

        _foodRepositoryMock.Setup(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Food?)null);

        _factsServiceMock.Setup(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FoodDto
            {
                Name = "Test Food",
                NutritionPer100G = new NutrimentsPer100GDto
                {
                    Calories = 330,
                    Protein = 10,
                    Fat = 30,
                    Carbohydrates = 5,
                    Salt = 8,
                    Sugar = 2,
                    SaturatedFat = 0,
                    Fiber = 0,
                    Sodium = 0,
                    Cholesterol = 0
                },
                Barcode = barcode,
                ImageUrl = null,
                Brand = null
            });


        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.IsSuccess).IsTrue();
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ValidRequestWithFoodId_ShouldAddFoodEntry()
    {
        var userId = UserId.New();
        var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
        var mealType = MealType.Lunch;
        var quantityInGrams = 100;

        var dailyLog = DailyLog.Create(dailyLogDate, null, 3000, userId);
        dailyLog.AddMeal(Meal.Initialize(mealType, userId));

        var food = Food.Create("Test Food", new NutritionPer100G(330, 10, 30, 5, 8, 2, 0), FoodDataSource.Database);

        var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, null, food.Id, quantityInGrams);

        _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));

        _dailyLogRepositoryMock.Setup(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);
        //
        // _foodRepositoryMock.Setup(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(food);


        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.IsSuccess).IsTrue();
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidUser_ShouldReturnNotFound()
    {
        var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
        var mealType = MealType.Lunch;
        var barcode = "1234567890123";
        var quantityInGrams = 100;

        var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, barcode, null, quantityInGrams);

        _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo("User profile not found.");
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_InvalidQuantityInGrams_ShouldReturnBadRequest()
    {
        var userId = UserId.New();
        var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
        var mealType = MealType.Lunch;
        var barcode = "1234567890123";
        var quantityInGrams = -1;

        var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, barcode, null, quantityInGrams);

        _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));

        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo("Quantity must be greater than zero.");
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_BarcodeAndFoodIdNull_ShouldReturnBadRequest()
    {
        var userId = UserId.New();
        var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
        var mealType = MealType.Lunch;
        var quantityInGrams = 100;

        var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, null, null, quantityInGrams);

        _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));

        var commandHandler = new AddFoodEntryCommand.Handler(
            _dailyLogRepositoryMock.Object,
            _profileRepositoryMock.Object,
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _factsServiceMock.Object,
            _unitOfWorkMock.Object);

        var result = await commandHandler.Handle(commandPayload, CancellationToken.None);

        _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo("Quantity must be greater than zero.");
        _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // [Test]
    // public async Task Handle_DailyLogNotFoundByDate_ShouldReturnNotFound()
    // {
    //     var userId = UserId.New();
    //     var dailyLogDate = DateOnly.FromDateTime(DateTime.Now);
    //     var mealType = MealType.Lunch;
    //     var barcode = "1234567890123";
    //     var quantityInGrams = 100;
    //
    //     var commandPayload = new AddFoodEntryCommand(dailyLogDate, mealType, barcode, null, quantityInGrams);
    //
    //     _profileRepositoryMock.Setup(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));
    //
    //     _dailyLogRepositoryMock.Setup(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(dailyLog);
    //
    //     _foodRepositoryMock.Setup(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(food);
    //
    //
    //     var commandHandler = new AddFoodEntryCommand.Handler(
    //         _dailyLogRepositoryMock.Object,
    //         _profileRepositoryMock.Object,
    //         _foodRepositoryMock.Object,
    //         _userServiceMock.Object,
    //         _factsServiceMock.Object,
    //         _unitOfWorkMock.Object);
    //
    //     var result = await commandHandler.Handle(commandPayload, CancellationToken.None);
    //
    //     _profileRepositoryMock.Verify(x => x.GetWithIncludesByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
    //     await Assert.That(result.IsSuccess).IsTrue();
    //     _dailyLogRepositoryMock.Verify(x => x.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Once);
    //     _foodRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<FoodId>(), It.IsAny<CancellationToken>()), Times.Never);
    //     _foodRepositoryMock.Verify(x => x.GetByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    //     _factsServiceMock.Verify(x => x.GetFoodByBarcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    //     _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
    //     _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    // }
}