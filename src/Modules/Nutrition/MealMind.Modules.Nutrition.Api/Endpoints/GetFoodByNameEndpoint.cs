using MealMind.Modules.Nutrition.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints;

public class GetFoodByNameEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("food/name/{name}",
            async (string name, ISender sender) =>
            {
                var result = await sender.Send(new GetFoodByNameQuery(name));
                return result;
            });
    }
}