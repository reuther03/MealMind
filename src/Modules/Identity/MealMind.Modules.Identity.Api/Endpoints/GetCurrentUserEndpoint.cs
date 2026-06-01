using MealMind.Modules.Identity.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints;

internal sealed class GetCurrentUserEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/identity/me",
                async (ISender sender) =>
                {
                    var result = await sender.Send(new GetCurrentUserQuery());
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Get Current User",
                "Returns the authenticated user's identity details (id, email, username) and current subscription tier.",
                "",
                """
                {
                  "value": {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "email": "john.doe@example.com",
                    "username": "johndoe",
                    "subscriptionTier": "Free"
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
