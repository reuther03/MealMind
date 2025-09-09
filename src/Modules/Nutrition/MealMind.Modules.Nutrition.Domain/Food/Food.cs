using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class Food : AggregateRoot<FoodId>
{
    private readonly List<Name> _tags = [];
    public Name Name { get; private set; }
    public string? Barcode { get; private set; }
    public string? NutritionPer100g { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Brand { get; private set; }
    public IReadOnlyList<Name> Tags => _tags.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsCustom { get; private set; }
    public string Source { get; private set; } // e.g., "OpenFoodFacts", "User", appSeeded
}