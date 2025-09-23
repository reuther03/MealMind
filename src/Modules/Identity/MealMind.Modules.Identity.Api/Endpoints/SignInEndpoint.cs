using MealMind.Modules.Identity.Application.Features.Commands.SignInCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints;

internal sealed class SignInEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("sign-in",
                async (SignInCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .AllowAnonymous()
            .WithDocumentation("Sign In",
                "Authenticates a user with email and password. Returns a JWT access token with user details.",
                """
                {
                  "email": "john.doe@example.com",
                  "password": "SecurePassword123!"
                }
                """,
                """
                {
                  "value": {
                    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "email": "john.doe@example.com",
                    "username": "johndoe"
                  },
                  "isSuccessful": true,
                  "errors": []
                }
                """
            );
    }
}