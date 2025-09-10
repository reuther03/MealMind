using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class Food : AggregateRoot<FoodId>
{
    private readonly List<FoodCategory> _categories = [];
    private readonly List<DietaryTag> _dietaryTags = [];
    public Name Name { get; private set; }
    public string? Barcode { get; private set; }
    public NutritionPer100G NutritionPer100G { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Brand { get; private set; }
    public IReadOnlyList<FoodCategory> Categories => _categories.AsReadOnly();
    public IReadOnlyList<DietaryTag> DietaryTags => _dietaryTags.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public bool IsCustom { get; private set; }
    public string Source { get; private set; } // e.g., "OpenFoodFacts", "User", appSeeded

    private Food()
    {
    }

    private Food(FoodId id, Name name, NutritionPer100G nutritionPer100G, string source) : base(id)
    {
        Name = name;
        NutritionPer100G = nutritionPer100G;
        Source = source;
        CreatedAt = DateTime.UtcNow;
        IsCustom = source == "User";
    }

    public static Food Create(Name name, NutritionPer100G nutritionPer100G, string source, string? barcode = null, string? imageUrl = null,
        string? brand = null, IEnumerable<FoodCategory>? categories = null, IEnumerable<DietaryTag>? tags = null)
    {
        var food = new Food(FoodId.New(), name, nutritionPer100G, source)
        {
            Barcode = barcode,
            ImageUrl = imageUrl,
            Brand = brand
        };

        if (categories != null)
            food._categories.AddRange(categories);

        if (tags != null)
            food._dietaryTags.AddRange(tags);

        return food;
    }
}