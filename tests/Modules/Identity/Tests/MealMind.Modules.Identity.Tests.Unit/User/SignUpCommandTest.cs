using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;
using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Nutrition;
using MealMind.Shared.Contracts.Types;
using MealMind.Shared.Events.Identity;
using Moq;

namespace MealMind.Modules.Identity.Tests.Unit.User;

public class SignUpCommandTest
{
    private readonly Mock<IIdentityUserRepository> _identityUserRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOutboxService> _outboxService = new();


    [Test]
    public async Task Handler_ValidData_ShouldSignUp()
    {
        var username = "Test User";
        var email = "test@test.com";
        var password = "Test123!";

        var nutritionTargets = new List<NutritionTargetPayload>
        {
            new(2000, null, new NutritionInPercentPayload
            {
                ProteinInPercent = 30,
                CarbohydratesInPercent = 30,
                FatsInPercent = 40
            }, 2, [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]),
            new(2200, null, new NutritionInPercentPayload
            {
                ProteinInPercent = 30,
                CarbohydratesInPercent = 30,
                FatsInPercent = 40
            }, 2.5m, [DayOfWeek.Thursday, DayOfWeek.Friday]),
            new(2500, null, new NutritionInPercentPayload
            {
                ProteinInPercent = 30,
                CarbohydratesInPercent = 30,
                FatsInPercent = 40
            }, 3, [DayOfWeek.Saturday, DayOfWeek.Sunday])
        };
        var personalData = new PersonalDataPayload(Gender.Male, DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), 180, 80, 75, ActivityLevel.Active);

        var commandPayload = new SignUpCommand(username, email, password, personalData, nutritionTargets);

        var commandHandler = new SignUpCommand.Handler(_identityUserRepository.Object, _unitOfWork.Object, _outboxService.Object);
        await commandHandler.Handle(commandPayload, CancellationToken.None);

        _identityUserRepository.Verify(x => x.ExistsWithEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _identityUserRepository.Verify(x => x.AddAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _outboxService.Verify(x => x.SaveAsync(It.IsAny<IdentityUserCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _outboxService.Verify(x => x.SaveAsync(It.IsAny<SubscriptionTierAddedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handler_InvalidEmail_ShouldReturnBadRequest()
    {
        var username = "Test User";
        var email = "test@test.com";
        var password = "Test123!";

        var nutritionTargets = new List<NutritionTargetPayload>
        {
            new(2000, null, new NutritionInPercentPayload
            {
                ProteinInPercent = 30,
                CarbohydratesInPercent = 30,
                FatsInPercent = 40
            }, 2, [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]),
            new(2200, null, new NutritionInPercentPayload
            {
                ProteinInPercent = 30,
                CarbohydratesInPercent = 30,
                FatsInPercent = 40
            }, 2.5m, [DayOfWeek.Thursday, DayOfWeek.Friday]),
            new(2500, null, new NutritionInPercentPayload
            {
                ProteinInPercent = 30,
                CarbohydratesInPercent = 30,
                FatsInPercent = 40
            }, 3, [DayOfWeek.Saturday, DayOfWeek.Sunday])
        };
        var personalData = new PersonalDataPayload(Gender.Male, DateOnly.FromDateTime(DateTime.Now.AddYears(-30)), 180, 80, 75, ActivityLevel.Active);

        var commandPayload = new SignUpCommand(username, email, password, personalData, nutritionTargets);

        _identityUserRepository
            .Setup(x => x.ExistsWithEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var commandHandler = new SignUpCommand.Handler(_identityUserRepository.Object, _unitOfWork.Object, _outboxService.Object);
        await commandHandler.Handle(commandPayload, CancellationToken.None);

        _identityUserRepository.Verify(x => x.ExistsWithEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _identityUserRepository.Verify(x => x.AddAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _outboxService.Verify(x => x.SaveAsync(It.IsAny<IdentityUserCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _outboxService.Verify(x => x.SaveAsync(It.IsAny<SubscriptionTierAddedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}