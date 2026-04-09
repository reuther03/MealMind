using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.SubscriptionDeletedCommand;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Commands;

public class SubscriptionDeletedCommandTest
{
    private readonly Mock<IIdentityUserRepository> _identityUserRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public SubscriptionDeletedCommandTest()
    {
        _identityUserRepositoryMock = new Mock<IIdentityUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Test]
    public async Task Handle_ValidRequest_ShouldCancelSubscription()
    {
        var user = IdentityUser.Create("Test User", "test@test.com", new Password("test123!"));
        var sub = user.Subscription.UpdateToPaidTier(SubscriptionTier.Standard, "cus_test123", "sub_test123", DateTime.UtcNow, DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1), "active");
        user.UpdateSubscription(sub);

        _identityUserRepositoryMock.Setup(x => x.GetUserByCustomerIdAsync(user.Subscription.StripeCustomerId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var commandHandler = new SubscriptionDeletedCommand.Handler(
            _identityUserRepositoryMock.Object,
            _unitOfWorkMock.Object
        );

        var result = await commandHandler.Handle(
            new SubscriptionDeletedCommand(user.Subscription.StripeCustomerId!, DateTime.UtcNow, "canceled"),
            CancellationToken.None
        );

        await Assert.That(result.IsSuccess).IsTrue();
        _identityUserRepositoryMock.Verify(x => x.GetUserByCustomerIdAsync(user.Subscription.StripeCustomerId!, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidUser_ShouldReturnNotFound()
    {
        var invalidCustomerId = "invalidUser";

        _identityUserRepositoryMock.Setup(x => x.GetUserByCustomerIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUser?)null);

        var commandHandler = new SubscriptionDeletedCommand.Handler(
            _identityUserRepositoryMock.Object,
            _unitOfWorkMock.Object
        );

        var result = await commandHandler.Handle(
            new SubscriptionDeletedCommand(invalidCustomerId, DateTime.UtcNow, "canceled"),
            CancellationToken.None
        );

        await Assert.That(result.IsSuccess).IsFalse();
        _identityUserRepositoryMock.Verify(x => x.GetUserByCustomerIdAsync(invalidCustomerId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

}