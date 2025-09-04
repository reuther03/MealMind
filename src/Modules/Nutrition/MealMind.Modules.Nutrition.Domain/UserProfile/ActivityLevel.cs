namespace MealMind.Modules.Nutrition.Domain.UserProfile;

public enum ActivityLevel
{
    Sedentary = 1,        // Little or no exercise, desk job 1.2
    Light = 2,            // Light exercise 1-3 days/week 1.375
    Moderate = 3,         // Moderate exercise 3-5 days/week 1.55
    Active = 4,           // Hard exercise 6-7 days/week 1.725
    VeryActive = 5        // Intense exercise or physical job 1.9
}