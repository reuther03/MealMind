using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public record PersonalData : ValueObject
{
    public Gender Gender { get; }
    public DateOnly DateOfBirth { get; }
    public decimal Weight { get; }
    public decimal Height { get; }
    public ActivityLevel ActivityLevel { get; }

    private PersonalData(Gender gender, DateOnly dateOfBirth, decimal weight, decimal height, ActivityLevel activityLevel)
    {
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Weight = weight;
        Height = height;
        ActivityLevel = activityLevel;
    }

    public static PersonalData Create(Gender gender, DateOnly dateOfBirth, decimal weight, decimal height, ActivityLevel activityLevel)
    {
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Date of birth cannot be in the future");

        if (weight is < 10 or > 400)
            throw new DomainException("Weight must be between 10 and 400");

        if (height is < 80 or > 250)
            throw new DomainException("Height must be between 80cm and 250cm");

        return new PersonalData(gender, dateOfBirth, weight, height, activityLevel);
    }


    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Gender;
        yield return DateOfBirth;
        yield return Weight;
        yield return Height;
        yield return ActivityLevel;
    }
}