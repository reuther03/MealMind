using MealMind.Modules.AiChat.Application.Features.Queries.GetConversationQuery;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints.Get;

public class GetConversationDetailsEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/conversation/{conversationId:guid}",
                async (Guid conversationId, ISender sender) =>
                {
                    var result = await sender.Send(new GetConversationQuery(conversationId));
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Get Conversation Details",
                "Retrieves a specific conversation with all its chat messages for the authenticated user. Messages are ordered by creation date. Requires authentication.",
                "GET /AiChat-module/conversation/3fa85f64-5717-4562-b3fc-2c963f66afa6",
                """
                {
                  "value": {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "title": "Healthy Meal Planning",
                    "chatMessages": [
                      {
                        "id": "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
                        "role": "User",
                        "content": "What are some healthy breakfast options?",
                        "replyToMessageId": null,
                        "createdAt": "2024-01-15T08:30:00Z"
                      },
                      {
                        "id": "2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e",
                        "role": "Assistant",
                        "content": "Here are some nutritious breakfast options: oatmeal with fruits, Greek yogurt with nuts, whole grain toast with avocado...",
                        "replyToMessageId": "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
                        "createdAt": "2024-01-15T08:30:15Z"
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
