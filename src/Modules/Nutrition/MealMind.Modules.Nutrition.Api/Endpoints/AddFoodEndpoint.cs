using MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints;

public class AddFoodEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("food/add",
                async (AddFoodCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Add Food to Meal",
                "Adds a food item to a specific meal in the user's daily log. If the food is from the database (has Id), it's fetched directly. If it's from OpenFoodFacts (has Barcode), it's fetched from the external API and saved to the database first. Creates the daily log if it doesn't exist. MealType: 0=Breakfast, 1=Lunch, 2=Dinner, 3=Snack.",
                """
                POST http://localhost:5000/food/add

                Example 1 - Adding food from database:
                {
                  "foodId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                  "barcode": null,
                  "quantityInGrams": 150.5,
                  "dailyLogDate": "2025-10-28",
                  "mealType": 0
                }

                Example 2 - Adding food from OpenFoodFacts:
                {
                  "foodId": null,
                  "barcode": "4061458003872",
                  "quantityInGrams": 200,
                  "dailyLogDate": "2025-10-28",
                  "mealType": 1
                }
                """,
                """
                {
                  "value": "8b3c7d92-4f6e-4a1c-9e2b-5f8a3c6d1e9f",
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}
