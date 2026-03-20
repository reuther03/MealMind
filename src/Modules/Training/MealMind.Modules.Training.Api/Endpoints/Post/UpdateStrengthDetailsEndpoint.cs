using MealMind.Modules.Training.Application.Features.Commands.UpdateStrengthDetails;
using MealMind.Modules.Training.Domain.TrainingPlan;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class UpdateStrengthDetailsEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPut("/training-plans/{planId:guid}/sessions/{sessionId:guid}/exercises/{orderIndex:int}/strength",
                async (Guid planId, Guid sessionId, int orderIndex, StrengthDetails strengthDetails,
                    ISender sender) =>
                {
                    var result = await sender.Send(new UpdateStrengthDetailsCommand(planId, sessionId, orderIndex, strengthDetails));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Update Strength Details",
                "Replaces the strength details (sets, reps, weight) for a specific exercise in a training session. Sends the complete list of sets - the backend overwrites the existing data.",
                """
                {
                  "strengthDetails": {
                    "sets": [
                      { "setNumber": 1, "repetitions": 12, "weight": 60.0, "setType": "Warmup", "restTimeInSeconds": 90 },
                      { "setNumber": 2, "repetitions": 10, "weight": 80.0, "setType": "Working", "restTimeInSeconds": 120 },
                      { "setNumber": 3, "repetitions": 8, "weight": 85.0, "setType": "Working", "restTimeInSeconds": 120 },
                      { "setNumber": 4, "repetitions": 6, "weight": 90.0, "setType": "DropSet", "restTimeInSeconds": null }
                    ]
                  }
                }
                """,
                """
                {
                  "value": true,
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}