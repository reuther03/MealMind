using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Contracts.Types;

namespace MealMind.Modules.Nutrition.Tests.Unit;

public class PersonalDataTest
{
    [Test]
    public async Task Create_ValidData_ShouldCreatePersonalData()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 50;
        var height = 180;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;

        var personalData = PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel);

        await Assert.That(personalData).IsNotNull();
    }

    [Test]
    public async Task Create_InvalidWeightAndHeight_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var invalidDateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
        var validWeight = 50;
        var invalidWeightToSmall = 8;
        var invalidWeightToBig = 500;
        var height = 180;
        var invalidHeight = 300;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, invalidDateOfBirth, validWeight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Date of birth cannot be in the future");

        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, invalidWeightToSmall, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");

        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, invalidWeightToBig, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");

        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, validWeight, height, invalidWeightToSmall, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");

        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, validWeight, height, invalidWeightToBig, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");

        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, validWeight, invalidHeight, validWeight, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Height must be between 80cm and 250cm");
    }
}