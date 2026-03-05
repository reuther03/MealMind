using MealMind.Modules.AiChat.Application.Features.Commands.CreateFoodWithAiCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.AiChat.Api.Endpoints;

public class CreateFoodWithAiEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/create-food-with-ai",
                async (
                    [FromForm] string foodPrompt,
                    IFormFile? imageFile,
                    ISender sender) =>
                {
                    var result = await sender.Send(new CreateFoodWithAiCommand(foodPrompt, imageFile));
                    return result;
                })
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithDocumentation("Create Food With AI",
                "Creates a new food item using AI based on a text description and optional image. The AI estimates nutritional values per 100g. Requires multipart/form-data.",
                """
                Form Data:
                - foodPrompt: "Grilled chicken breast" (required - text description of the food)
                - imageFile: <multipart_file> (optional - JPEG/PNG image of the food)
                """,
                """
                {
                  "value": true,
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
