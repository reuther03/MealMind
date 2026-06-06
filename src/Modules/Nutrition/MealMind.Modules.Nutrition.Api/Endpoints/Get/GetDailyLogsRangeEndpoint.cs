using MealMind.Modules.Nutrition.Application.Features.Queries.GetDailyLogsRangeQuery;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Get;

public class GetDailyLogsRangeEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/dailylog/range",
                async (DateOnly startDate, DateOnly endDate, ISender sender) =>
                {
                    var result = await sender.Send(new GetDailyLogsRangeQuery(startDate, endDate));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Get Daily Logs Range",
                "Returns the authenticated user's daily nutrition logs within a date range (inclusive), plus aggregate summary: total days in range, days logged, days on calorie target (±10% margin), nutrition totals and per-logged-day averages. Empty range returns 200 OK with zero counters and empty list.",
                "GET http://localhost:5000/dailylog/range?startDate=2026-05-25&endDate=2026-05-31",
                """
                {
                  "value": {
                    "startDate": "2026-05-25",
                    "endDate": "2026-05-31",
                    "totalDaysInRange": 7,
                    "daysLogged": 6,
                    "daysOnTarget": 4,
                    "rangeTotals": {
                      "calories": 12450,
                      "proteins": 720,
                      "carbohydrates": 1380,
                      "fats": 410,
                      "fiber": 195
                    },
                    "dailyAverages": {
                      "calories": 2075,
                      "proteins": 120,
                      "carbohydrates": 230,
                      "fats": 68.3
                    },
                    "dailyLogs": [ /* per-day DailyLogDto entries, ordered by date */ ]
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
