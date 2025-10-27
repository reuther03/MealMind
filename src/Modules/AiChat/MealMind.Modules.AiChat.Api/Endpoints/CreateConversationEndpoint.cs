using MealMind.Modules.AiChat.Application.Features.Commands.CreateConversationCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class CreateConversationEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("create-chat-response",
            async (CreateConversationCommand request, ISender sender) =>
            {
                var result = await sender.Send(request);
                return result;
            })
            .WithDocumentation("Create Chat Conversation",
                "Creates a new AI chat conversation with an initial user prompt. Returns the AI's response to the first message. Requires authentication.",
                """
                {
                  "prompt": "I want to start eating healthier. Can you suggest a meal plan for someone trying to lose weight?"
                }
                """,
                """
                {
                  "value": "I'd be happy to help you with a healthy meal plan for weight loss! Here's a balanced approach...",
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}