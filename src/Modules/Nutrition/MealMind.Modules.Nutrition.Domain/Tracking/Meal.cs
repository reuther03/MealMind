using MealMind.Modules.Nutrition.Domain.Food;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public class Meal : Entity<Guid>
{
    private readonly List<FoodEntry> _foods = [];

    public MealType MealType { get; private set; }
    public Name? Name { get; private set; }
    public IReadOnlyCollection<FoodEntry> Foods => _foods.AsReadOnly();
    public decimal TotalCalories => _foods.Sum(f => f.TotalCalories);
    public decimal TotalProteins => _foods.Sum(f => f.TotalProteins);
    public decimal TotalCarbohydrates => _foods.Sum(f => f.TotalCarbohydrates);
    public decimal TotalSugars => _foods.Sum(f => f.TotalSugars);
    public decimal TotalFats => _foods.Sum(f => f.TotalFats);
    public decimal TotalSaturatedFats => _foods.Sum(f => f.TotalSaturatedFats);
    public decimal TotalFiber => _foods.Sum(f => f.TotalFiber);
    public decimal TotalSodium => _foods.Sum(f => f.TotalSodium);
    public UserId UserId { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
    public string? Notes { get; private set; }

    private Meal()
    {
    }

    private Meal(Guid id, MealType mealType, UserId userId, Name? name = null, DateTime? consumedAt = null, string? notes = null)
        : base(id)
    {
        MealType = mealType;
        UserId = userId;
        Name = name ?? nameof(mealType);
        ConsumedAt = consumedAt;
        Notes = notes;
    }

    public static Meal Create(Guid id, MealType mealType, UserId userId, Name? name = null, DateTime? consumedAt = null, string? notes = null)
        => new(id, mealType, userId, name, consumedAt, notes);

    public void AddFood(FoodEntry foodEntry)
        => _foods.Add(foodEntry);
}