using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Contracts.Dto.Nutrition;

namespace MealMind.Modules.Nutrition.Domain.Food;

public class Food : AggregateRoot<FoodId>
{
    private readonly List<Category> _categories = [];
    private readonly List<DietaryTag> _dietaryTags = [];
    public Name Name { get; private set; }
    public string? Barcode { get; private set; }
    public NutritionPer100G NutritionPer100G { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Brand { get; private set; }
    public IReadOnlyList<Category> Categories => _categories.AsReadOnly();
    public IReadOnlyList<DietaryTag> DietaryTags => _dietaryTags.AsReadOnly();
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
        string? brand = null)
    {
        var food = new Food(FoodId.New(), name, nutritionPer100G, foodDataSource)
        {
            Barcode = barcode,
            ImageUrl = imageUrl,
            Brand = brand
        };

        return food;
    }

    public void AssignTags(IEnumerable<Category> cats, IEnumerable<DietaryTag> tags)
    {
        _categories.Clear();
        _categories.AddRange(cats);

        _dietaryTags.Clear();
        _dietaryTags.AddRange(tags);
    }

    public void UpdateStatistics(FoodStatistics statistics)
        => Statistics = statistics;
}