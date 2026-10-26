using MealMind.Modules.Training.Application.Features.Commands.StartTrainingSessionCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class StartTrainingSessionEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/training-plans/{planId:guid}/sessions/{previousSessionId:guid}/start",
                async (Guid planId, Guid previousSessionId, ISender sender) =>
                {
                    var result = await sender.Send(new StartTrainingSessionCommand(planId, previousSessionId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Start Training Session",
                "Starts a new training session by cloning a source session (a template or a completed session) and adds it to the specified training plan. The source session cannot be in progress.",
                "",
                """
                {
                  "value": true,
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """);
    }
}
