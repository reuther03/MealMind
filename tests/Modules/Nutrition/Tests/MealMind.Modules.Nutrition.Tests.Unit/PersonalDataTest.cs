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
    public async Task Create_FutureDateOfBirth_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
        var weight = 50;
        var height = 180;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Date of birth cannot be in the future");
    }

    [Test]
    public async Task Create_WeightTooSmall_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 9;
        var height = 180;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");
    }

    [Test]
    public async Task Create_WeightTooBig_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 500;
        var height = 180;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");
    }

    [Test]
    public async Task Create_WeightTargetTooSmall_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 50;
        var height = 180;
        var weightTarget = 9;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");
    }

    [Test]
    public async Task Create_WeightTargetTooBig_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 50;
        var height = 180;
        var weightTarget = 600;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Weight must be between 10 and 400");
    }

    [Test]
    public async Task Create_HeightTooSmall_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 50;
        var height = 79;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Height must be between 80cm and 250cm");
    }

    [Test]
    public async Task Create_HeightTooTall_ShouldThrow()
    {
        var gender = Gender.Male;
        var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var weight = 50;
        var height = 251;
        var weightTarget = 60;
        var activityLevel = ActivityLevel.Active;


        await Assert.That(() => PersonalData.Create(gender, dateOfBirth, weight, height, weightTarget, activityLevel))
            .Throws<DomainException>()
            .WithMessage("Height must be between 80cm and 250cm");
    }
}