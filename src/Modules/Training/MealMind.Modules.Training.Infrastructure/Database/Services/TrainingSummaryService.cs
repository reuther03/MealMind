// using MealMind.Shared.Abstractions.Exception;
// using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
// using MealMind.Shared.Abstractions.Services;
// using Microsoft.EntityFrameworkCore;
//
// namespace MealMind.Modules.Training.Infrastructure.Database.Services;
//
// internal class TrainingSummaryService : ITrainingSummaryService
// {
//     private readonly TrainingDbContext _dbContext;
//
//     public TrainingSummaryService(TrainingDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public async Task<string> BuildSummaryAsync(UserId userId, CancellationToken ct)
//     {
//         var trainingPlan = await _dbContext.TrainingPlans
//             .Include(x => x.Sessions)
//             .ThenInclude(x => x.Exercises)
//             .FirstOrDefaultAsync(x => x.UserId == userId, ct);
//
//         if (trainingPlan == null)
//             throw new DomainException("Training plan not found.");
//
//         var days = 14;
//         var today = DateOnly.FromDateTime(DateTime.UtcNow);
//         var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
//         var mondayThisWeek = today.AddDays(-daysFromMonday);
//         var sundayLastWeek = mondayThisWeek.AddDays(-1);
//     }
// }