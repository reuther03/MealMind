namespace MealMind.Modules.Nutrition.Domain.Food;

public enum Category
{
    // Protein sources
    Meat = 1,
    Poultry,
    Fish,
    Seafood,
    Dairy,
    Yogurt,
    Cheese,
    Eggs,
    PlantProtein, // e.g., tofu, tempeh, seitan

    // Carbohydrates
    Grains,
    Bread,
    Pasta,
    Rice,
    Cereals,
    Legumes, // beans, lentils, chickpeas
    Potatoes,
    SweetPotatoes,

    // Vegetables & Fruits
    Vegetables,
    LeafyGreens,
    Fruits,
    Berries,
    Nuts,
    Seeds,

    // Fats & Oils
    Oils,
    Butters,
    Avocados,

    // Beverages
    Water,
    Coffee,
    Tea,
    Juice,
    Soda,
    Alcohol,

    // Snacks & Sweets
    Chocolate,
    Candy,
    Biscuits,
    Chips,
    IceCream,

    // Prepared Meals
    Soup,
    Salad,
    FastFood,
    FrozenMeal,
    RestaurantMeal,

    // Supplements
    ProteinPowder,
    Creatine,
    Vitamins,

    // Other / Misc
    Condiments,
    Sauces,
    Spices,
    Other
}