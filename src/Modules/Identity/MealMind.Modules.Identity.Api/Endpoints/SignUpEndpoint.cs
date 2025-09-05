using MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints;

internal sealed class SignUpEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("sign-up",
                async (SignUpCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return Results.Ok(result);
                })
            .AllowAnonymous()
            .WithDocumentation("Sign Up",
                "Creates a new user account with username, email, and password. Returns the new user's ID.",
                """
                {
                  "username": "johndoe",
                  "email": "john.doe@example.com",
                  "inputPassword": "SecurePassword123!"
                }
                """,
                """
                {
                  "value": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                  "isSuccessful": true,
                  "errors": []
                }
                """);
    }
}