using MealMind.Modules.Nutrition.Application.Features.Queries;
using MealMind.Modules.Nutrition.Application.Features.Queries.GetDailyLogQuery;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Get;

public class GetDailyLogEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("dailylog",
                async (DateOnly dateOnly, ISender sender) =>
                {
                    var result = await sender.Send(new GetDailyLogQuery(dateOnly));
                    return result;
                })
            .AllowAnonymous()
            .WithDocumentation(
                "Get Daily Log",
                "Retrieves the daily nutrition log for a specified date, including meals and their nutritional breakdown. Useful for tracking daily food intake and nutritional goals.",
                "GET http://localhost:5000/dailylog?dateOnly=2024-01-01",
                """
                {
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}