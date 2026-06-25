using MealMind.Modules.Training.Application.Features.Commands.CreatePlanCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Post;

public class CreatePlanEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/training-plans",
                async (CreatePlanCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Create Training Plan",
                "Creates a new training plan for the authenticated user. A plan is a thin container grouping sessions by a planned weekday (e.g. \"Monday Push\"). The plan is active on creation; sessions are added separately via CreateTrainingSession.",
                """
                {
                  "name": "Monday Push",
                  "plannedAt": "Monday"
                }
                """,
                """
                {
                  "value": "7d1f4c8b-2a36-4e9d-9c81-5d2f0b3a7c41",
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
