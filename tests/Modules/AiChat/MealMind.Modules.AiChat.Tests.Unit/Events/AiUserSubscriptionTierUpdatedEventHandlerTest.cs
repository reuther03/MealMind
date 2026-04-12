using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Events.Integration;
using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Events.Identity;
using Moq;

namespace MealMind.Modules.AiChat.Tests.Unit.Events;

public class AiUserSubscriptionTierUpdatedEventHandlerTest
{
    private readonly Mock<IAiChatUserRepository> _aiChatUserRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AiUserSubscriptionTierUpdatedEventHandlerTest()
    {
        _aiChatUserRepositoryMock = new Mock<IAiChatUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task Handle_ValidUser_ShouldUpdateSubscriptionTier()
    {
        var subscriptionTier = SubscriptionTier.Premium;
        var user = AiChatUser.Create(UserId.New());

        _aiChatUserRepositoryMock.Setup(x => x.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var eventHandler = new AiUserSubscriptionTierUpdatedEventHandler(_aiChatUserRepositoryMock.Object, _unitOfWorkMock.Object);

        await eventHandler.Handle(new SubscriptionTierUpdatedEvent(user.Id, subscriptionTier), CancellationToken.None);

        _aiChatUserRepositoryMock.Verify(x => x.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _aiChatUserRepositoryMock.Verify(x => x.Update(It.IsAny<AiChatUser>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_UserNotFound_ShouldThrow()
    {
        var subscriptionTier = SubscriptionTier.Premium;
        var userId = UserId.New();

        _aiChatUserRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AiChatUser)null!);

        var eventHandler = new AiUserSubscriptionTierUpdatedEventHandler(_aiChatUserRepositoryMock.Object, _unitOfWorkMock.Object);

        await eventHandler.Handle(new SubscriptionTierUpdatedEvent(userId, subscriptionTier), CancellationToken.None);

        _aiChatUserRepositoryMock.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _aiChatUserRepositoryMock.Verify(x => x.Update(It.IsAny<AiChatUser>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}