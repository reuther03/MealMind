namespace MealMind.Modules.Nutrition.Domain.Food;

public enum DietaryTag
{
    // Diet Preferences
    Vegan = 1, // No animal products
    Vegetarian, // No meat, but may include dairy/eggs
    Pescatarian, // Fish allowed, no other meat
    Keto, // Low-carb, high-fat
    Paleo, // Whole foods, no processed grains/dairy
    LowCarb, // Reduced carbohydrates
    HighProtein, // Focus on protein-rich foods
    LowFat, // Reduced fat content
    LowCalorie, // Suitable for calorie-restricted diets

    // Allergen-Free Options
    GlutenFree,
    DairyFree,
    LactoseFree,
    NutFree,
    PeanutFree,
    SoyFree,
    EggFree,
    ShellfishFree,
    FishFree,
    SesameFree,

    // Other Lifestyle Preferences
    Organic,
    NonGmo,
    SugarFree,
    LowSugar,
    CaffeineFree,
    WholeGrain,
    HighFiber,
}