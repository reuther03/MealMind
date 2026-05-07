using MealMind.Modules.AiChat.Application.Features.Commands.SaveFoodAnalyzeCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class SaveFoodAnalyzeEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/save-food-analyze",
                async (SaveFoodAnalyzeCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Save Food Analyze",
                "Saves the result of a previously analyzed food image (or one of its corrections) to the user's daily log. EstimationMode picks which value to use from the min/max range: 0=Minimal, 1=Average, 2=Maximal.",
                """
                {
                  "sessionId": "8b1f6a3e-2c4f-4a90-9c7e-6b1f2e3d4a5b",
                  "correctionId": null,
                  "estimationMode": 1,
                  "logDate": "2026-05-08"
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
