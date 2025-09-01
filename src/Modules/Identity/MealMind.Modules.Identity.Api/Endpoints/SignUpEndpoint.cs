using MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Stoxly.Shared.Abstractions.Api;

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
            .AllowAnonymous();
    }
}