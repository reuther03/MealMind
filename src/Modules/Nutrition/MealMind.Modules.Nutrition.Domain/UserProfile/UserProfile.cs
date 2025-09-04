using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class UserProfile : AggregateRoot<UserId>
{
    private readonly List<NutritionTarget> _nutritionTargets = [];
    public Name Username { get; private set; }
    public Email Email { get; private set; }
    public PersonalData PersonalData { get; private set; }
    public IReadOnlyList<NutritionTarget> NutritionTargets => _nutritionTargets.AsReadOnly();

    private UserProfile()
    {
    }

    private UserProfile(UserId id, Name userName, Email email) : base(id)
    {
        Username = userName;
        Email = email;
    }

    public static UserProfile Create(UserId id, Name userName, Email email)
        => new(id, userName, email);

    public void UpdatePersonalData(PersonalData personalData)
        => PersonalData = personalData;

    public void AddNutritionTarget(NutritionTarget nutritionTarget)
        => _nutritionTargets.Add(nutritionTarget);
}