using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Services;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.User;

public class SignUpCommandTest
{
    private readonly Mock<IIdentityUserRepository> _identityUserRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOutboxService> _outboxService = new();


    [Test]
    public async Task Handler_ValidData_ShouldSignUpSuccessfully()
    {
        // var username = "Test User";
        // var email = "test@test.com";
        // var password = "Test123!";
        //
        // var user = IdentityUser.Create(username, email, password);
        //
        // var commandHandler = new SignUpCommand.Handler(_identityUserRepository.Object, _unitOfWork.Object, _outboxService.Object);
        // await eventHandler.Handle(eventPayload, CancellationToken.None);
        //
        // _userProfileRepository.Verify(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}