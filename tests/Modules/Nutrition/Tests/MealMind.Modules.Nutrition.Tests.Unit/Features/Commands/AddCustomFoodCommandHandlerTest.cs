using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Features.Commands.AddCustomFoodCommand;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.Messaging.AiChat;
using MealMind.Shared.Abstractions.QueriesAndCommands.Requests;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Result;
using Microsoft.Extensions.Logging;
using Moq;

namespace MealMind.Modules.Nutrition.Tests.Unit.Features.Commands;

public class AddCustomFoodCommandHandlerTest
{
    private readonly Mock<IFoodRepository> _foodRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUserProfileRepository> _profileRepositoryMock;
    private readonly Mock<ILogger<AddCustomFoodCommand>> _loggerMock;
    private readonly Mock<ISender> _senderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private static readonly NutritionPer100G SampleNutrition = new(400, 20, 40, 15, null);
    private const string FoodName = "Chicken Breast";
    private const string FoodBrand = "FarmCo";

    public AddCustomFoodCommandHandlerTest()
    {
        _foodRepositoryMock = new Mock<IFoodRepository>();
        _userServiceMock = new Mock<IUserService>();
        _profileRepositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<AddCustomFoodCommand>>();
        _senderMock = new Mock<ISender>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _userServiceMock.Setup(x => x.UserId).Returns(UserId.New());
    }

    private AddCustomFoodCommand.Handler CreateHandler() =>
        new(
            _foodRepositoryMock.Object,
            _userServiceMock.Object,
            _profileRepositoryMock.Object,
            _loggerMock.Object,
            _senderMock.Object,
            _unitOfWorkMock.Object);

    private void SetupUserProfileFound()
    {
        var userId = _userServiceMock.Object.UserId!;
        _profileRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free));
    }

    [Test]
    public async Task Handle_WhenUserProfileNotFound_ShouldReturnBadRequestAndNeverCallAiOrRepository()
    {
        // Arrange
        _profileRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo("User profile not found.");
        _senderMock.Verify(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenBothTagsProvidedByUser_ShouldSkipAiCallAndSaveFoodWithProvidedTags()
    {
        // Arrange
        SetupUserProfileFound();

        var categories = new List<Category> { Category.Poultry };
        var dietaryTags = new List<DietaryTag> { DietaryTag.HighProtein, DietaryTag.GlutenFree };

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, categories, dietaryTags);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsNotEqualTo(Guid.Empty);
        _senderMock.Verify(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenCategoriesProvidedButDietaryTagsEmpty_ShouldSkipAiCallAndSaveFood()
    {
        // Arrange
        SetupUserProfileFound();

        var categories = new List<Category> { Category.Poultry };
        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, categories, []);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        _senderMock.Verify(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenBothTagsEmptyAndAiReturnsValidTags_ShouldParsTagsAndSaveFood()
    {
        // Arrange
        SetupUserProfileFound();

        var aiResult = Result<FoodTagsResult>.Ok(
            new FoodTagsResult(
                Categories: ["Poultry", "HighProtein"],
                DietaryTags: ["GlutenFree", "HighProtein"]));

        _senderMock
            .Setup(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResult);

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsNotEqualTo(Guid.Empty);
        _senderMock.Verify(
            x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenBothTagsEmptyAndAiReturnsValidTags_ShouldSendQueryWithCorrectProductNameAndBrand()
    {
        // Arrange
        SetupUserProfileFound();

        var aiResult = Result<FoodTagsResult>.Ok(
            new FoodTagsResult(
                Categories: ["Poultry"],
                DietaryTags: ["HighProtein"]));

        GenerateFoodTagsQuery? capturedQuery = null;
        _senderMock
            .Setup(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<FoodTagsResult>>, CancellationToken>((q, _) =>
                capturedQuery = q as GenerateFoodTagsQuery)
            .ReturnsAsync(aiResult);

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(capturedQuery).IsNotNull();
        await Assert.That(capturedQuery!.ProductName).IsEqualTo(FoodName);
        await Assert.That(capturedQuery.Brand).IsEqualTo(FoodBrand);
    }

    [Test]
    public async Task Handle_WhenBothTagsEmptyAndAiTagsAreAllUnparseable_ShouldReturnBadRequestAndNotSaveFood()
    {
        // Arrange
        SetupUserProfileFound();

        var aiResult = Result<FoodTagsResult>.Ok(
            new FoodTagsResult(
                Categories: ["NotARealCategory", "???"],
                DietaryTags: ["AlsoGarbage", "!!"]));

        _senderMock
            .Setup(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResult);

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo(
            "Could not infer tags for this food. Please provide at least one category or dietary tag.");
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenBothTagsEmptyAndAiReturnsFailureResult_ShouldReturnBadRequestAndNotSaveFood()
    {
        // Arrange
        SetupUserProfileFound();

        var aiResult = Result<FoodTagsResult>.BadRequest("AI service rejected the request.");

        _senderMock
            .Setup(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResult);

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo(
            "Automatic tagging is currently unavailable. Please provide at least one category or dietary tag.");
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenBothTagsEmptyAndSenderThrowsException_ShouldLogWarningAndReturnBadRequest()
    {
        // Arrange
        SetupUserProfileFound();

        _senderMock
            .Setup(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Connection refused"));

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Message).IsEqualTo(
            "Automatic tagging is currently unavailable. Please provide at least one category or dietary tag.");
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenBothTagsEmptyAndSenderThrowsOperationCanceledException_ShouldRethrowAndNotSaveFood()
    {
        // Arrange
        SetupUserProfileFound();

        _senderMock
            .Setup(x => x.Send(It.IsAny<IRequest<Result<FoodTagsResult>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var command = new AddCustomFoodCommand(
            FoodName, null, SampleNutrition, "http://img.test/img.jpg", FoodBrand, null, null);

        var handler = CreateHandler();

        // Act & Assert
        await Assert.That(async () => await handler.Handle(command, CancellationToken.None))
            .Throws<OperationCanceledException>();

        _foodRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Food>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}