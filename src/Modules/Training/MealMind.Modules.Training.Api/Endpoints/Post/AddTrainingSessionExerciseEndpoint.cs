using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class AddTrainingSessionExerciseEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/training-plans/{planId:guid}/sessions/{sessionId:guid}/exercises",
                async (Guid planId, Guid sessionId, AddExerciseToSessionRequest request,
                    ITrainingPlanRepository trainingPlanRepository,
                    IExerciseRepository exerciseRepository,
                    IUserService userService,
                    IUnitOfWork unitOfWork,
                    CancellationToken cancellationToken) =>
                {
                    if (!userService.IsAuthenticated)
                        return Result.BadRequest("User is not authenticated.");

                    var trainingPlan = await trainingPlanRepository.GetByIdAsync(planId, userService.UserId, cancellationToken);
                    if (trainingPlan is null)
                        return Result.NotFound("Training plan not found.");

                    var session = trainingPlan.Sessions.FirstOrDefault(s => s.Id == sessionId);
                    if (session is null)
                        return Result.NotFound("Session not found.");

                    var exercise = await exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken);
                    if (exercise is null)
                        return Result.NotFound("Exercise not found.");

                    var lastOrderIndex = session.Exercises.Any()
                        ? session.Exercises.Max(x => x.OrderIndex)
                        : 0;

                    var sessionExercise = exercise.Type == ExerciseType.Strength
                        ? SessionExercise.Create(exercise.Id, lastOrderIndex + 1, new StrengthDetails(), null, request.Notes)
                        : SessionExercise.Create(exercise.Id, lastOrderIndex + 1, null, CardioDetails.CreateEmpty(), request.Notes);

                    session.AddExercise(sessionExercise);
                    await unitOfWork.CommitAsync(cancellationToken);

                    return Result.Ok();
                })
            .RequireAuthorization()
            .WithDocumentation("Add Exercise To Session",
                "Adds an exercise from the catalog to a training session. Automatically assigns order index and creates empty strength/cardio details based on exercise type.",
                """
                {
                  "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                  "notes": "Focus on form"
                }
                """,
                """
                {
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}

public record AddExerciseToSessionRequest(Guid ExerciseId, string? Notes);