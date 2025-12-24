using MealMind.Modules.AiChat.Application.Features.Queries.GetConversationsQuery;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints.Get;

public class GetConversationsEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/get-conversations",
                async (ISender sender, [FromQuery] int page = 1) =>
                {
                    var result = await sender.Send(new GetConversationsQuery(page));
                    return result;
                })
            .WithDocumentation(
                "Get Chat Conversations",
                "Retrieves a paginated list of chat conversations for the authenticated user. Requires authentication.",
                """
                {
                  "page": 1
                }
                """,
                """
                {
                  "value": {
                    "currentPage": 1,
                    "pageSize": 10,
                    "totalCount": 25,
                    "totalPages": 3,
                    "items": [
                      {
                        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "userId": "1c6b147e-8f3b-4d2a-9f7e-2c963f66afa6",
                        "title": "Healthy Dinner Ideas",
                        "createdAt": "2024-01-15T10:30:00Z",
                        "updatedAt": "2024-01-16T12:45:00Z"
                      },
                      {
                        "id": "4fa85f64-5717-4562-b3fc-2c963f66afb7",
                        "userId": "1c6b147e-8f3b-4d2a-9f7e-2c963f66afa6",
                        "title": "Vegan Recipes",
                        "createdAt": "2024-01-10T09:20:00Z",
                        "updatedAt": "2024-01-11T11:15:00Z"
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