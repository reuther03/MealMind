using MealMind.Modules.Nutrition.Application.Features.Commands;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints;

internal sealed class SetPersonalDataEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("personal-data",
                async (SetPersonalDataCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return Result.Ok(result);
                })
            .RequireAuthorization()
            .WithDocumentation("Set Personal Data",
                "Sets the personal data for the authenticated user.",
                """
                    {
                     test: "data"
                    }
                """
            )
            .WithTags(nameof(NutritionModule));
    }
}