// using MealMind.Shared.Abstractions.Kernel.Primitives;
// using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
//
// namespace MealMind.Modules.Nutrition.Domain.Tracking;
//
// public class WeekLog : AggregateRoot<WeekLogId>
// {
//     private readonly List<DailyLog> _dailyLogs = [];
//
//     public DateOnly StartDate { get; private set; }
//     public DateOnly EndDate { get; private set; }
//     public IReadOnlyList<DailyLog> DailyLogs => _dailyLogs.AsReadOnly();
//     public UserId UserId { get; private set; }
//
//     public decimal AverageWeight
//     {
//         get { return _dailyLogs.Count == 0 ? 0 : _dailyLogs.Average(x => x.CurrentWeight); }
//         private set { }
//     }
// }