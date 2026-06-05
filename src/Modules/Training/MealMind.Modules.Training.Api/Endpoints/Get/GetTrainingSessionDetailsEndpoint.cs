using MealMind.Modules.Training.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Get;

public class GetTrainingSessionDetailsEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/training-plans/{planId:guid}/sessions/{sessionId:guid}",
                async (Guid planId, Guid sessionId, ISender sender) =>
                {
                    var result = await sender.Send(new GetTrainingSessionDetailsQuery(planId, sessionId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Get Training Session Details",
                "Returns full details of a single training session: metadata, ordered list of exercises with embedded catalog info (name, image, type, muscle group) and per-exercise strength/cardio details with sets.",
                "",
                """
                {
                  "value": {
                    "id": "a3b8e2f4-9d11-4c27-8e45-2f6c1b0d9a82",
                    "name": "Push Day",
                    "description": "Chest + shoulders + triceps",
                    "startedAt": "2026-06-01T08:30:00Z",
                    "endedAt": "2026-06-01T09:45:00Z",
                    "isStarted": true,
                    "isCompleted": true,
                    "exercises": [
                      {
                        "id": "b41c9d3e-5f72-4a18-9b04-7e3c2a8f6d11",
                        "orderIndex": 1,
                        "exercise": {
                          "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                          "name": "Bench Press",
                          "imageUrl": "https://cdn.example.com/bench.jpg",
                          "type": "Strength",
                          "muscleGroup": "Chest",
                          "isCustom": false
                        },
                        "strengthDetails": {
                          "exerciseSets": [
                            { "setNumber": 1, "repetitions": 10, "weight": 60, "setType": "Warmup", "restTimeInSeconds": 90 },
                            { "setNumber": 2, "repetitions": 8, "weight": 80, "setType": "Working", "restTimeInSeconds": 120 },
                            { "setNumber": 3, "repetitions": 6, "weight": 85, "setType": "Working", "restTimeInSeconds": 120 }
                          ]
                        },
                        "cardioDetails": null
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
