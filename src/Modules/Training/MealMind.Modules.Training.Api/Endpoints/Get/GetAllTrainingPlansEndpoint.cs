using MealMind.Modules.Training.Application.Features.Queries;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Training.Api.Endpoints.Get;

public class GetAllTrainingPlansEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/training-plans",
                async (int? page, int? pageSize, ISender sender) =>
                {
                    var result = await sender.Send(new GetAllTrainingPlansQuery(
                        page ?? 1,
                        pageSize ?? 10));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Get All Training Plans",
                "Returns a paginated list of the authenticated user's training plans. Each item is a slim summary (no nested sessions/exercises). Sorted by last completed session (most recent first), then by name.",
                """
                Query parameters:
                - page: 1 (default)
                - pageSize: 10 (default)
                """,
                """
                {
                  "value": {
                    "page": 1,
                    "pageSize": 10,
                    "totalCount": 3,
                    "items": [
                      {
                        "id": "7d1f4c8b-2a36-4e9d-9c81-5d2f0b3a7c41",
                        "name": "Monday Push",
                        "plannedOn": "Monday",
                        "isActive": true,
                        "sessionsCount": 8,
                        "lastCompletedSessionAt": "2026-06-01T09:45:00Z"
                      },
                      {
                        "id": "b3a6c9e1-4d28-4f7b-8c52-1a9d3e0f5b62",
                        "name": "Wednesday Pull",
                        "plannedOn": "Wednesday",
                        "isActive": true,
                        "sessionsCount": 6,
                        "lastCompletedSessionAt": "2026-05-29T18:22:00Z"
                      }
                    ]
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
