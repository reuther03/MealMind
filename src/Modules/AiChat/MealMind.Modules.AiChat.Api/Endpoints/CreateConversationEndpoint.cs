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
            .RequireAuthorization()
            .WithDocumentation("Create Chat Conversation",
                "Creates a new AI chat conversation with an initial user prompt. Returns the AI's response to the first message. Requires authentication.",
                """
                {
                  "prompt": "I want to start eating healthier. Can you suggest a meal plan for someone trying to lose weight?"
                }
                """,
                """
                {
                 "value": {
                 "title": "Healthy Meal Plan for Weight Loss",
                 "paragraphs": [
                    "Here is a simple meal plan to help you get started on your journey to healthier eating and weight loss...",
                    "Breakfast: Oatmeal with fresh berries and a sprinkle of chia seeds...",
                    "Lunch: Grilled chicken salad with mixed greens, cherry tomatoes, cucumbers, and a light vinaigrette...",
                    "Dinner: Baked salmon with steamed broccoli and quinoa...",
                    "Snacks: Greek yogurt with honey, a handful of almonds, or carrot sticks with hummus..."
                 ],
                 "keyPoints": [
                    "Incorporate a variety of fruits and vegetables into your meals.",
                    "Choose lean proteins like chicken, fish, and legumes.",
                    "Opt for whole grains over refined grains.",
                    "Stay hydrated by drinking plenty of water throughout the day.",
                    "Limit processed foods and sugary snacks."
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