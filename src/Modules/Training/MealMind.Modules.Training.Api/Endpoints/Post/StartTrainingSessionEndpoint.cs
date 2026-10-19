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
                "Starts a new training session based on a previous completed session and adds it to the specified training plan. The previous session must be completed.",
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
