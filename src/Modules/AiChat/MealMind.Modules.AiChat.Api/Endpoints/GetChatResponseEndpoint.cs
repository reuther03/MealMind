using MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class GetChatResponseEndpoint : EndpointBase
{

    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/get-chat-response",
                async (GetChatResponseCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .WithDocumentation(
                "Get Chat Response",
                "Sends a message to an existing conversation and receives a structured AI response with RAG (Retrieval-Augmented Generation). The response includes relevant sources from the knowledge base. Requires authentication.",
                """
                {
                  "conversationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                  "prompt": "What are good protein sources for vegetarians?"
                }
                """,
                """
                {
                  "value": {
                    "title": "Vegetarian Protein Sources",
                    "paragraphs": [
                      "Vegetarians have many excellent protein options available...",
                      "Legumes such as lentils, chickpeas, and beans are outstanding sources..."
                    ],
                    "keyPoints": [
                      "Legumes provide 15-20g protein per cup",
                      "Quinoa is a complete protein with all essential amino acids",
                      "Greek yogurt and cottage cheese are high in protein"
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