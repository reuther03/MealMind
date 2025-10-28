using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Modules.Nutrition.Domain.Tracking;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public class UserProfile : AggregateRoot<UserId>
{
    private readonly List<WeightHistory> _weightHistory = [];
    private readonly List<NutritionTarget> _nutritionTargets = [];
    private readonly List<FoodId> _favoriteFoods = [];
    private readonly List<MealId> _favoriteMeals = [];
    public Name Username { get; private set; }
    public Email Email { get; private set; }
    public PersonalData PersonalData { get; private set; }
    public IReadOnlyList<WeightHistory> WeightHistory => _weightHistory.AsReadOnly();
    public IReadOnlyList<NutritionTarget> NutritionTargets => _nutritionTargets.AsReadOnly();
    public IReadOnlyList<FoodId> FavoriteFoods => _favoriteFoods.AsReadOnly();
    public IReadOnlyList<MealId> FavoriteMeals => _favoriteMeals.AsReadOnly();


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

    public void SetPersonalData(PersonalData personalData)
        => PersonalData = personalData;

    public void AddNutritionTarget(NutritionTarget nutritionTarget)
    {
        if (_nutritionTargets.Any(x => x.ActiveDays.Any(z => nutritionTarget.ActiveDays.Select(c => c.DayOfWeek).Contains(z.DayOfWeek))))
            throw new InvalidOperationException("A nutrition target with overlapping active days already exists.");

        _nutritionTargets.Add(nutritionTarget);
    }

    public WeightHistory GetWeightHistory(DateOnly date)
    {
        var weightEntry = _weightHistory.FirstOrDefault(w => w.Date == date);

        return weightEntry == null
            // for testing purposes we allow weight 0 if no entry exists
            ? Domain.UserProfile.WeightHistory.Create(Id, date, 10)
            : weightEntry;
    }

    public void AddFavoriteFood(FoodId foodId)
    {
        if (_favoriteFoods.Contains(foodId))
            throw new InvalidOperationException("Food is already in favorites.");

        _favoriteFoods.Add(foodId);
    }

    public void AddFavoriteMeal(Guid mealId)
    {
        if (_favoriteMeals.Contains(mealId))
            throw new InvalidOperationException("Meal is already in favorites.");

        _favoriteMeals.Add(mealId);
    }

    public void AddWeightHistory(WeightHistory weightHistory)
    {
        if (_weightHistory.Any(w => w.Date == weightHistory.Date))
            throw new InvalidOperationException("Weight entry for this date already exists.");

        _weightHistory.Add(weightHistory);
    }
}