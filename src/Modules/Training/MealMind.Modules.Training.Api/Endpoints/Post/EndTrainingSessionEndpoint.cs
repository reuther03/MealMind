using MealMind.Modules.Training.Application.Features.Commands.EndTrainingSessionCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class EndTrainingSessionEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/training-plans/{planId:guid}/sessions/{sessionId:guid}/end",
                async (Guid planId, Guid sessionId, ISender sender) =>
                {
                    var result = await sender.Send(new EndTrainingSessionCommand(planId, sessionId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("End Training Session",
                "Marks a started training session as ended and returns a comparison against the previous completed session in the same plan. If there is no previous session (first session in the plan), the response includes current stats but Previous/PreviousSessionId/deltas are null.",
                "",
                """
                {
                  "value": {
                    "sessionId": "a3b8e2f4-9d11-4c27-8e45-2f6c1b0d9a82",
                    "previousSessionId": "1c7f9d2a-4b8e-4f3c-8a1d-9e5b0c6f2d44",
                    "endedAt": "2026-06-26T09:45:00Z",
                    "previousEndedAt": "2026-06-19T09:30:00Z",
                    "current": {
                      "exercisesCount": 5,
                      "totalSets": 18,
                      "totalVolume": 12450,
                      "cardioDurationInMinutes": 0,
                      "cardioCaloriesBurned": 0
                    },
                    "previous": {
                      "exercisesCount": 5,
                      "totalSets": 17,
                      "totalVolume": 11800,
                      "cardioDurationInMinutes": 0,
                      "cardioCaloriesBurned": 0
                    },
                    "exercises": [
                      {
                        "exerciseName": "Bench Press",
                        "exerciseType": "Strength",
                        "currentBestWeight": 85,
                        "currentBestReps": 5,
                        "previousBestWeight": 82.5,
                        "previousBestReps": 5,
                        "weightDelta": 2.5,
                        "repsDelta": 0,
                        "currentVolume": 2125,
                        "previousVolume": 1980,
                        "volumeDelta": 145,
                        "currentDurationInMinutes": null,
                        "previousDurationInMinutes": null,
                        "durationDelta": null,
                        "currentDistanceInKm": null,
                        "previousDistanceInKm": null,
                        "distanceDelta": null,
                        "currentCaloriesBurned": null,
                        "previousCaloriesBurned": null,
                        "caloriesBurnedDelta": null
                      }
                    ]
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
