using MealMind.Modules.AiChat.Application.Features.Commands.CreateConversationCommand;
using MealMind.Modules.AiChat.Application.Features.Commands.GetChatResponseCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class CreateConversationEndpoint : EndpointBase
{
    //create should return id of created conversation
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("create-chat-response",
            async (CreateConversationCommand request, ISender sender) =>
            {
                var result = await sender.Send(request);
                return result;
            });
    }
}