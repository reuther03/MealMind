using MealMind.Modules.Nutrition.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints;

public class GetFoodByNameEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("food/search",
            async ([AsParameters] GetFoodByNameRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetFoodByNameQuery(request.SearchTerm, request.PageSize, request.Page));
                return result;
            });
    }
}

public record GetFoodByNameRequest(string SearchTerm, int PageSize = 10, int Page = 1);