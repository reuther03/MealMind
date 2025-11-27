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
                    [FromForm] string? prompt,
                    [FromForm] NutritionEstimationMode estimationMode,
                    IFormFile image,
                    [FromForm] DateOnly? dailyLogDate,
                    [FromForm] bool saveFoodEntry,
                    ISender sender) =>
                {
                    var result = await sender.Send(new GetCaloriesFromImageCommand(prompt, estimationMode, image, dailyLogDate, saveFoodEntry));
                    return result;
                })
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithDocumentation("Get Calories From Image",
                "Gets calorie information based on an image and user prompt. Requires authentication.",
                """
                {
                  "prompt": "Please analyze the nutritional content of this meal. // can be null",
                  "image": "<image_file_here>"
                }
                """,
                """
                {
                "value": " The meal contains approximately 600 calories, including 20g of protein, 50g of carbohydrates, and 25g of fat.",
                "isSuccess": true,
                "statusCode": 200,
                "message": null
                }
                """
            );
    }
}