using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Abstractions.Services;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.CreateCheckoutSessionCommand;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.Services;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.Stripe.Commands;

public class CreateCheckoutSessionCommandTest
{
    private readonly Mock<IStripeService> _stripeServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IIdentityUserRepository> _identityUserRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateCheckoutSessionCommandTest()
    {
        _stripeServiceMock = new Mock<IStripeService>();
        _userServiceMock = new Mock<IUserService>();
        _identityUserRepositoryMock = new Mock<IIdentityUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _userServiceMock.Setup(x => x.UserId).Returns(UserId.New());
    }

    [Test]
    public async Task Handle_ValidRequest_ShouldCreateCheckoutSession()
    {
        var subscriptionTier = SubscriptionTier.Standard;
        var userId = _userServiceMock.Object.UserId!;
        var user = IdentityUser.Create("Test User", "testuser@test.com", new Password("test123!"));

        _identityUserRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var commandHandler = new CreateCheckoutSessionCommand.Handler(
            _stripeServiceMock.Object,
            _userServiceMock.Object,
            _identityUserRepositoryMock.Object,
            _unitOfWorkMock.Object
        );

        var result = await commandHandler.Handle(new CreateCheckoutSessionCommand(subscriptionTier), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        _stripeServiceMock.Verify(x => x.CreateCheckoutSessionAsync(user.Id, subscriptionTier), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidUser_ShouldReturnNotFound()
    {
        var subscriptionTier = SubscriptionTier.Standard;
        var userId = _userServiceMock.Object.UserId!;

        _identityUserRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUser?)null);

        var commandHandler = new CreateCheckoutSessionCommand.Handler(
            _stripeServiceMock.Object,
            _userServiceMock.Object,
            _identityUserRepositoryMock.Object,
            _unitOfWorkMock.Object
        );

        var result = await commandHandler.Handle(new CreateCheckoutSessionCommand(subscriptionTier), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        _stripeServiceMock.Verify(x => x.CreateCheckoutSessionAsync(It.IsAny<UserId>(), It.IsAny<SubscriptionTier>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}