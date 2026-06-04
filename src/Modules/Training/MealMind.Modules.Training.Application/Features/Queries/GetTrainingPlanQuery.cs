// using MealMind.Modules.Training.Application.Abstractions.Database;
// using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
// using MealMind.Shared.Contracts.Dto.Training;
// using MealMind.Shared.Contracts.Result;
//
// namespace MealMind.Modules.Training.Application.Features.Queries;
//
// public class GetTrainingPlanQuery : IQuery<TrainingPlanDto>
// {
//     public sealed class Handler : IQueryHandler<GetTrainingPlanQuery, TrainingPlanDto>
//     {
//         private readonly ITrainingDbContext _dbContext;
//
//         public Handler(ITrainingDbContext dbContext)
//         {
//             _dbContext = dbContext;
//         }
//
//         public Task<Result<TrainingPlanDto>> Handle(GetTrainingPlanQuery request, CancellationToken cancellationToken)
//         {
//         }
//     }
// }