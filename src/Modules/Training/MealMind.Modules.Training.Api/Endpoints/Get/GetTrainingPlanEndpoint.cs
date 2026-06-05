using MealMind.Modules.Training.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Get;

public class GetTrainingPlanEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/training-plans/{planId:guid}",
                async (Guid planId, ISender sender) =>
                {
                    var result = await sender.Send(new GetTrainingPlanDetailsQuery(planId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Get Training Plan",
                "Returns full details of an active training plan owned by the authenticated user, including its sessions and per-session exercises with strength/cardio details.",
                "",
                """
                {
                  "value": {
                    "id": "7d1f4c8b-2a36-4e9d-9c81-5d2f0b3a7c41",
                    "name": "Monday Push",
                    "plannedOn": "Monday",
                    "isActive": true,
                    "sessions": [
                      {
                        "id": "a3b8e2f4-9d11-4c27-8e45-2f6c1b0d9a82",
                        "name": "Push Day",
                        "description": "Chest + shoulders + triceps",
                        "startedAt": "2026-06-01T08:30:00Z",
                        "endedAt": "2026-06-01T09:45:00Z",
                        "exercises": [
                          {
                            "id": "b41c9d3e-5f72-4a18-9b04-7e3c2a8f6d11",
                            "orderIndex": 1,
                            "strengthDetails": {
                              "exerciseSets": [
                                { "setNumber": 1, "repetitions": 10, "weight": 80, "setType": "Working", "restTimeInSeconds": 120 },
                                { "setNumber": 2, "repetitions": 8, "weight": 85, "setType": "Working", "restTimeInSeconds": 120 }
                              ]
                            },
                            "cardioDetails": null
                          }
                        ]
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
