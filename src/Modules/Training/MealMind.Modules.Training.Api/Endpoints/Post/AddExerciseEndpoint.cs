using MealMind.Modules.Training.Application.Features.Commands.AddExerciseCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class AddExerciseEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/training-plans/{planId:guid}/sessions/{sessionId:guid}/exercises",
                async (Guid planId, Guid sessionId, AddExerciseRequest request, ISender sender) =>
                {
                    var result = await sender.Send(new AddExerciseCommand(planId, sessionId, request.ExerciseId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Add Exercise To Session",
                "Adds an exercise from the catalog to a training session. The session exercise is created with empty details that match the catalog exercise type (StrengthDetails for Strength, CardioDetails for Cardio/Other). Details are filled in afterwards via the update endpoints.",
                """
                {
                  "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
                }
                """,
                """
                {
                  "value": "9c2b4d1a-7e3f-4f6c-bd24-1f6e0b8a4d33",
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}

public record AddExerciseRequest(Guid ExerciseId);
