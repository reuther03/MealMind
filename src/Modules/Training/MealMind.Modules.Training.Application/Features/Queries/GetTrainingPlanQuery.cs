using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetTrainingPlanQuery(Guid Id) : IQuery<TrainingPlanDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingPlanQuery, TrainingPlanDto>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(ITrainingDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<TrainingPlanDto>> Handle(GetTrainingPlanQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.IsAuthenticated)
                return Result<TrainingPlanDto>.BadRequest("User is not authenticated.");

            var userId = _userService.UserId;

            var trainingPlan = await _dbContext.TrainingPlans
                .Where(x => x.Id == TrainingPlanId.From(request.Id) && x.UserId == userId && x.IsActive)
                .Select(x => new TrainingPlanDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PlannedOn = x.PlannedOn,
                    IsActive = x.IsActive,
                    Sessions = x.Sessions.Select(s => new TrainingSessionDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        StartedAt = s.StartedAt,
                        EndedAt = s.EndedAt,
                        Description = s.Description,
                        Exercises = s.Exercises.Select(e => new SessionExerciseDto
                        {
                            Id = e.Id,
                            OrderIndex = e.OrderIndex,
                            StrengthDetails = e.StrengthDetails == null
                                ? null
                                : new StrengthDetailsDto
                                {
                                    ExerciseSets = e.StrengthDetails.Sets.Select(es => new ExerciseSetDto
                                    {
                                        SetNumber = es.SetNumber,
                                        Repetitions = es.Repetitions,
                                        Weight = es.Weight,
                                        SetType = es.SetType.ToString(),
                                        RestTimeInSeconds = es.RestTimeInSeconds
                                    }).ToList()
                                },
                            CardioDetails = e.CardioDetails == null
                                ? null
                                : new CardioDetailsDto
                                {
                                    DurationInMinutes = e.CardioDetails.DurationInMinutes,
                                    DistanceInKm = e.CardioDetails.DistanceInKm,
                                    CaloriesBurned = e.CardioDetails.CaloriesBurned,
                                    AverageHeartRate = e.CardioDetails.AverageHeartRate,
                                    AverageSpeed = e.CardioDetails.AverageSpeed,
                                    Notes = e.CardioDetails.Notes,
                                    CaloriesEstimated = e.CardioDetails.CaloriesEstimated
                                }
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return trainingPlan == null ? Result<TrainingPlanDto>.NotFound("Training plan not found.") : Result<TrainingPlanDto>.Ok(trainingPlan);
        }
    }
}