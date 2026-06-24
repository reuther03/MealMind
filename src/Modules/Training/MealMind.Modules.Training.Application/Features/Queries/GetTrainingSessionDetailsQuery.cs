using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.QueriesAndCommands.Queries;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.Training;
using MealMind.Shared.Contracts.Result;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Modules.Training.Application.Features.Queries;

public record GetTrainingSessionDetailsQuery(Guid PlanId, Guid TrainingSessionId) : IQuery<TrainingSessionDetailsDto>
{
    public sealed class Handler : IQueryHandler<GetTrainingSessionDetailsQuery, TrainingSessionDetailsDto>
    {
        private readonly ITrainingDbContext _dbContext;
        private readonly IUserService _userService;

        public Handler(ITrainingDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Result<TrainingSessionDetailsDto>> Handle(GetTrainingSessionDetailsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userService.UserId;

            var session = await _dbContext.TrainingPlans
                .Where(p => p.Id == TrainingPlanId.From(request.PlanId) && p.UserId == userId && p.IsActive)
                .SelectMany(p => p.Sessions)
                .Where(s => s.Id == request.TrainingSessionId)
                .Select(s => new TrainingSessionDetailsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt,
                    Exercises = s.Exercises
                        .OrderBy(e => e.OrderIndex)
                        .Select(e => new SessionExerciseDto
                        {
                            Id = e.Id,
                            OrderIndex = e.OrderIndex,
                            Exercise = new ExerciseDto
                                {
                                    Id = e.ExerciseId,
                                    Name = e.Exercise.Name,
                                    ImageUrl = e.Exercise.ImageUrl,
                                    Type = e.Exercise.Type.ToString(),
                                    MuscleGroup = e.Exercise.MuscleGroup != null ? e.Exercise.MuscleGroup.ToString() : null,
                                    IsCustom = e.Exercise.IsCustom
                                },
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
                })
                .FirstOrDefaultAsync(cancellationToken);

            return session == null
                ? Result<TrainingSessionDetailsDto>.NotFound("Training session not found.")
                : Result<TrainingSessionDetailsDto>.Ok(session);
        }
    }
}