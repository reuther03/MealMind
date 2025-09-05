using MealMind.Modules.Identity.Application.Features.Commands.SignInCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
                    return Result.Ok(result);
                })
            .AllowAnonymous()
            .WithDocumentation(
                "Sign In",
                "Authenticates a user and returns a JWT token.",
                """
                {
                    "email":"
                }
                """);
    }
}