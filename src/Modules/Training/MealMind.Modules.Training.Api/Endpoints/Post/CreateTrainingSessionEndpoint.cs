using MealMind.Modules.Training.Application.Features.Commands.CreateTrainingSessionCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class CreateTrainingSessionEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/training-plans/{planId:guid}/sessions",
                async (Guid planId, CreateTrainingSessionRequest request, ISender sender) =>
                {
                    var result = await sender.Send(new CreateTrainingSessionCommand
                        (planId, request.Name, request.Description));

                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Create Training Session",
                "Creates a new training session for the specified training plan. A session is a container for exercises that are performed together in a single workout. The session is not started on creation; it can be started via the StartTrainingSession endpoint.",
                """
                {
                  "name": "Chest and Triceps",
                  "description": "A session focused on chest and triceps exercises."
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

public record CreateTrainingSessionRequest(string Name, string Description);
