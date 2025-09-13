// using MealMind.Modules.Nutrition.Domain.Food;
// using MealMind.Shared.Abstractions.Kernel.Primitives;
//
// namespace MealMind.Modules.Nutrition.Domain.Tracking;
//
// public class DailyLog : AggregateRoot<DailyLogId>
// {
//     private readonly List<Meal> _meals = [];
//     public DateTime Date { get; private set; }
//
//     public decimal CurrentWeight { get; private set; }
//
//     public IReadOnlyList<Meal> Meals => _meals.AsReadOnly();
//     // prop that sets calories goal but it is setted from users nutrition plan active days
// }