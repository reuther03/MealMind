using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.UpdateSubscriptionTierCommand;
using MealMind.Modules.Identity.Application.Features.Payloads;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Events.Identity;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Commands;

public class UpdateSubscriptionTierCommandTest
{
    private readonly Mock<IIdentityUserRepository> _identityUserRepositoryMock;
    private readonly Mock<IOutboxService> _outboxServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateSubscriptionTierCommandTest()
    {
        _identityUserRepositoryMock = new Mock<IIdentityUserRepository>();
        _outboxServiceMock = new Mock<IOutboxService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task Handle_ValidRequest_ShouldUpdateSubscription()
    {
        var user = IdentityUser.Create("Test User", "test@test.com", new Password("test123!"));
        var customerId = "cus_test123";
        var subId = "sub_test123";
        var subscriptionStartedAt = DateTime.UtcNow;
        var currentPeriodStart = DateTime.UtcNow;
        var currentPeriodEnd = DateTime.UtcNow.AddMonths(1);
        var subscriptionStatus = "active";


        _identityUserRepositoryMock.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var commandHandler = new UpdateSubscriptionTierCommand.Handler(
            _identityUserRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _outboxServiceMock.Object
        );

        var result = await commandHandler.Handle(
            new UpdateSubscriptionTierCommand(new UpdateSubscriptionTierPayload(
                user.Id,
                SubscriptionTier.Standard,
                customerId,
                subId,
                subscriptionStartedAt,
                currentPeriodStart,
                currentPeriodEnd,
                subscriptionStatus)), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        _identityUserRepositoryMock.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _outboxServiceMock.Verify(x => x.SaveAsync(It.IsAny<SubscriptionTierUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidUser_ShouldReturnNotFound()
    {
        var userId = Guid.NewGuid();
        var customerId = "cus_test123";
        var subId = "sub_test123";
        var subscriptionStartedAt = DateTime.UtcNow;
        var currentPeriodStart = DateTime.UtcNow;
        var currentPeriodEnd = DateTime.UtcNow.AddMonths(1);
        var subscriptionStatus = "active";

        _identityUserRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUser?)null);

        var commandHandler = new UpdateSubscriptionTierCommand.Handler(
            _identityUserRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _outboxServiceMock.Object
        );

        var result = await commandHandler.Handle(
            new UpdateSubscriptionTierCommand(new UpdateSubscriptionTierPayload(
                userId,
                SubscriptionTier.Standard,
                customerId,
                subId,
                subscriptionStartedAt,
                currentPeriodStart,
                currentPeriodEnd,
                subscriptionStatus)), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        _identityUserRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _outboxServiceMock.Verify(x => x.SaveAsync(It.IsAny<SubscriptionTierUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}