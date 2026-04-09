using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.UpdateSubscriptionAfterPaymentCommand;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Events.Identity;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Commands;

public class UpdateSubscriptionAfterPaymentCommandTest
{
    private readonly Mock<IIdentityUserRepository> _identityUserRepositoryMock;
    private readonly Mock<IOutboxService> _outboxServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateSubscriptionAfterPaymentCommandTest()
    {
        _identityUserRepositoryMock = new Mock<IIdentityUserRepository>();
        _outboxServiceMock = new Mock<IOutboxService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task Handle_ValidRequest_ShouldUpdateSubscription()
    {
        var user = IdentityUser.Create("Test User", "test@test.com", new Password("test123!"));
        var sub = user.Subscription.UpdateToPaidTier(SubscriptionTier.Standard, "cus_test123", "sub_test123", DateTime.UtcNow, DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1), "active");
        user.UpdateSubscription(sub);

        _identityUserRepositoryMock.Setup(x => x.GetUserBySubscriptionIdAsync(sub.StripeSubscriptionId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var commandHandler = new UpdateSubscriptionAfterPaymentCommand.Handler(
            _identityUserRepositoryMock.Object,
            _outboxServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var result = await commandHandler.Handle(
            new UpdateSubscriptionAfterPaymentCommand(sub.StripeSubscriptionId!, SubscriptionTier.Standard, DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(1), "active"),
            CancellationToken.None
        );

        await Assert.That(result.IsSuccess).IsTrue();
        _identityUserRepositoryMock.Verify(x => x.GetUserBySubscriptionIdAsync(sub.StripeSubscriptionId!, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _outboxServiceMock.Verify(x => x.SaveAsync(It.IsAny<SubscriptionTierUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidUser_ShouldReturnNotFound()
    {
        var subId = "sub_test123";

        _identityUserRepositoryMock.Setup(x => x.GetUserBySubscriptionIdAsync(subId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUser?)null);

        var commandHandler = new UpdateSubscriptionAfterPaymentCommand.Handler(
            _identityUserRepositoryMock.Object,
            _outboxServiceMock.Object,
            _unitOfWorkMock.Object
        );

        var result = await commandHandler.Handle(
            new UpdateSubscriptionAfterPaymentCommand(subId, SubscriptionTier.Premium, DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(1), "active"),
            CancellationToken.None
        );

        await Assert.That(result.IsSuccess).IsFalse();
        _identityUserRepositoryMock.Verify(x => x.GetUserBySubscriptionIdAsync(subId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _outboxServiceMock.Verify(x => x.SaveAsync(It.IsAny<SubscriptionTierUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}