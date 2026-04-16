using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Events.Integration;
using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Moq;

namespace MealMind.Modules.Nutrition.Tests.Unit.Events;

public class ImageAnalyzeCreatedEventTest
{
    private readonly Mock<IDailyLogRepository> _dailyLogRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public ImageAnalyzeCreatedEventTest()
    {
        _dailyLogRepository = new Mock<IDailyLogRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task Handle_ValidData_ShouldCreateImageAnalyze()
    {
        var userId = UserId.New();
        var foodName = "Test Food";
        var quantityInGrams = 100m;
        var totalCalories = 200m;
        var totalProteins = 10m;
        var totalCarbohydrates = 20m;
        var totalFats = 5m;
        var dailyLogDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var dailyLog = DailyLog.Create(dailyLogDate, null, 3000, userId);
        var meal = Meal.Initialize(MealType.Breakfast, userId);
        dailyLog.AddMeal(meal);

        _dailyLogRepository.Setup(x => x.GetByDateAsync(dailyLogDate, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailyLog);

        var handler = new ImageAnalyzeCreatedEventHandler(_dailyLogRepository.Object, _unitOfWork.Object,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<ImageAnalyzeCreatedEventHandler>>());

        await handler.Handle(
            new Shared.Events.AiChat.ImageAnalyzeCreatedEvent(userId, foodName, quantityInGrams, totalCalories, totalProteins, totalCarbohydrates, totalFats,
                dailyLogDate), CancellationToken.None);

        _dailyLogRepository.Verify(x => x.GetByDateAsync(dailyLogDate, userId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_DailyLogNotFound_ShouldNotCreateImageAnalyze()
    {
        var userId = UserId.New();
        var foodName = "Test Food";
        var quantityInGrams = 100m;
        var totalCalories = 200m;
        var totalProteins = 10m;
        var totalCarbohydrates = 20m;
        var totalFats = 5m;
        var dailyLogDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _dailyLogRepository.Reset();
        _dailyLogRepository.Setup(x => x.GetByDateAsync(dailyLogDate, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DailyLog?)null);

        var handler = new ImageAnalyzeCreatedEventHandler(_dailyLogRepository.Object, _unitOfWork.Object,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<ImageAnalyzeCreatedEventHandler>>());

        await handler.Handle(
            new Shared.Events.AiChat.ImageAnalyzeCreatedEvent(userId, foodName, quantityInGrams, totalCalories, totalProteins, totalCarbohydrates, totalFats,
                dailyLogDate), CancellationToken.None);

        _dailyLogRepository.Verify(x => x.GetByDateAsync(dailyLogDate, userId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}