using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class Food : AggregateRoot<FoodId>
{
    //todo: it is needed to be implemented categries and dietary tags
    private readonly List<FoodCategory> _categories = [];
    private readonly List<FoodDietaryTag> _dietaryTags = [];
    public Name Name { get; private set; }
    public string? Barcode { get; private set; }
    public NutritionPer100G NutritionPer100G { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Brand { get; private set; }
    public IReadOnlyList<FoodCategory> Categories => _categories.AsReadOnly();
    public IReadOnlyList<FoodDietaryTag> DietaryTags => _dietaryTags.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public FoodDataSource FoodDataSource { get; private set; }
    public FoodStatistics Statistics { get; private set; }

    private Food()
    {
    }

    private Food(FoodId id, Name name, NutritionPer100G nutritionPer100G, FoodDataSource foodDataSource) : base(id)
    {
        Name = name;
        NutritionPer100G = nutritionPer100G;
        FoodDataSource = foodDataSource;
        CreatedAt = DateTime.UtcNow;
    }

    public static Food Create(Name name, NutritionPer100G nutritionPer100G, FoodDataSource foodDataSource, string? barcode = null, string? imageUrl = null,
        string? brand = null, IEnumerable<FoodCategory>? categories = null, IEnumerable<FoodDietaryTag>? tags = null)
    {
        var food = new Food(FoodId.New(), name, nutritionPer100G, foodDataSource)
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

    public void UpdateStatistics(FoodStatistics statistics)
        => Statistics = statistics;
}