using MealMind.Modules.AiChat.Application.Features.Commands.AnalyzeNutritionCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class AnalyzeNutritionEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/{conversationId:guid}/analyze-nutrition",
                async ([FromRoute] Guid conversationId, [FromBody] AnalyzeNutritionCommand command, ISender sender) =>
                {
                    var result = await sender.Send(command with { ConversationId = conversationId });
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Analyze Nutrition",
                "Generates a structured AI evaluation of the user's logged nutrition data for the last 1-4 complete weeks (Monday-Sunday). The user's question is answered against a nutrition report built from their daily logs and targets, enriched with relevant RAG documents. Requires authentication.",
                """
                {
                  "prompt": "Am I on track to hit my weight target? Where am I slipping?",
                  "weeks": 2
                }
                """,
                """
                {
                  "value": {
                    "title": "Calorie Adherence Slipping Despite Weight Loss",
                    "paragraphs": [
                      "Across the last 14 days you averaged 2180 kcal/day and lost 1.20 kg (-1.41%), trending toward your 78 kg target.",
                      "Adherence weakened in the recent week: on-target days dropped from 6/7 (86%) to 3/7 (43%), with average calories rising by ~130 kcal."
                    ],
                    "keyPoints": [
                      "14/14 days logged - data is reliable",
                      "Weight -1.20 kg toward 78 kg target",
                      "On-target adherence fell 86% -> 43% week-over-week",
                      "Recent week averaged ~130 kcal higher than prior"
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
