using MealMind.Modules.AiChat.Application.Features.Commands.GetCaloriesFromImageCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class GetCaloriesFromImageEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("get-calories-from-image",
                async (
                    [FromForm] Guid sessionId,
                    [FromForm] string? prompt,
                    [FromForm] EstimationMode estimationMode,
                    IFormFile image,
                    [FromForm] DateOnly dailyLogDate,
                    [FromForm] bool saveFoodEntry,
                    ISender sender) =>
                {
                    var result = await sender.Send(new GetCaloriesFromImageCommand(sessionId, prompt, estimationMode, image));
                    return result;
                })
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithDocumentation("Get Calories From Image",
                "Analyzes food image using AI vision model and returns detailed nutrition estimates. Can optionally save to user's daily log. EstimationMode: 0=Minimal (conservative), 1=Average (balanced), 2=Maximal (aggressive). Requires multipart/form-data.",
                """
                Form Data:
                - prompt: "Analyze this burger meal" (optional)
                - estimationMode: 1 (required - 0=Minimal, 1=Average, 2=Maximal)
                - dailyLogDate: "2025-11-26" (required - date to save food entry to)
                - saveFoodEntry: true (required - whether to save to daily log)
                - image: <multipart_file> (required - JPEG/PNG image)
                """,
                """
                {
                  "value": {
                    "detectedFoods": [
                      {
                        "foodName": "Cheeseburger",
                        "minEstimatedCalories": 450,
                        "maxEstimatedCalories": 650,
                        "minEstimatedProteins": 20,
                        "maxEstimatedProteins": 30,
                        "minEstimatedCarbohydrates": 35,
                        "maxEstimatedCarbohydrates": 45,
                        "minEstimatedFats": 18,
                        "maxEstimatedFats": 28,
                        "quantityInGrams": 250,
                        "confidenceScore": 0.92
                      }
                    ],
                    "totalMinEstimatedCalories": 450,
                    "totalMaxEstimatedCalories": 650,
                    "totalMinEstimatedProteins": 20,
                    "totalMaxEstimatedProteins": 30,
                    "totalMinEstimatedCarbohydrates": 35,
                    "totalMaxEstimatedCarbohydrates": 45,
                    "totalMinEstimatedFats": 18,
                    "totalMaxEstimatedFats": 28,
                    "totalConfidenceScore": 0.92,
                    "totalQuantityInGrams": 250,
                    "foodName": "Cheeseburger",
                    "userDescription": "Analyze this burger meal"
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}