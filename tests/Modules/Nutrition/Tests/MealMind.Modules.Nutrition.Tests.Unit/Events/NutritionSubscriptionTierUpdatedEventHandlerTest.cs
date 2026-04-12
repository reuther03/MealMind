using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Events.Integration;
using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Events.Identity;
using Moq;

namespace MealMind.Modules.Nutrition.Tests.Unit.Events;

public class NutritionSubscriptionTierUpdatedEventHandlerTest
{
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public NutritionSubscriptionTierUpdatedEventHandlerTest()
    {
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task Handle_ValidUser_ShouldUpdateSubscriptionTier()
    {
        var subscriptionTier = SubscriptionTier.Premium;
        var userId = UserId.New();
        var userProfile = UserProfile.Create(userId, "Test User", "test@test.com", SubscriptionTier.Free);

        _userProfileRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        var eventHandler = new NutritionSubscriptionTierUpdatedEventHandler(
            _userProfileRepositoryMock.Object, _unitOfWorkMock.Object);

        await eventHandler.Handle(new SubscriptionTierUpdatedEvent(userId, subscriptionTier), CancellationToken.None);

        _userProfileRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userProfileRepositoryMock.Verify(x => x.Update(It.IsAny<UserProfile>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_UserNotFound_ShouldReturnWithoutUpdating()
    {
        var subscriptionTier = SubscriptionTier.Premium;
        var userId = UserId.New();

        _userProfileRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var eventHandler = new NutritionSubscriptionTierUpdatedEventHandler(
            _userProfileRepositoryMock.Object, _unitOfWorkMock.Object);

        await eventHandler.Handle(new SubscriptionTierUpdatedEvent(userId, subscriptionTier), CancellationToken.None);

        _userProfileRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userProfileRepositoryMock.Verify(x => x.Update(It.IsAny<UserProfile>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
