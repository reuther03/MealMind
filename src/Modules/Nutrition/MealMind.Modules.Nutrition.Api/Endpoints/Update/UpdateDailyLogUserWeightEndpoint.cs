using MealMind.Modules.Nutrition.Application.Features.Commands.UpdateDailyLogUserWeight;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Update;

public class UpdateDailyLogUserWeightEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPatch("daily-log/user-weight",
                async (UpdateDailyLogUserWeight request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Update Daily Log User Weight",
                "Updates the user's weight for a specific daily log date.",
                """
                {
                    "dailyLogDate": "2024-01-15",
                    "currentWeight": 75.5
                }
                """,
                """
                {
                  "value": true,
                  "isSuccessful": true,
                  "errors": []
                }
                """
            );
    }
}