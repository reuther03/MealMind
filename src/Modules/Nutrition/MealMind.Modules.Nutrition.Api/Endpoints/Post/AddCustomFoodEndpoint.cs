using MealMind.Modules.Nutrition.Application.Features.Commands.AddCustomFoodCommand;
using MealMind.Modules.Nutrition.Application.Features.Commands.AddFoodCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Post;

public class AddCustomFoodEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("food",
                async (AddCustomFoodCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Add New Custom Food",
                "Adds a new custom food item to the user's food database. The food item includes details such as name, barcode, nutritional information per 100 grams, image URL, brand, categories, and dietary tags.",
                """
                POST http://localhost:5000/food

                {
                  "name": "Homemade Granola",
                  "barcode": "1234567890123",
                  "nutritionPer100G": {
                    "calories": 450,
                    "proteinInGrams": 10,
                    "carbohydratesInGrams": 60,
                    "fatsInGrams": 15,
                    "fiberInGrams": 8,
                    "sugarsInGrams": 20,
                    "sodiumInMilligrams": 200
                  },
                  "imageUrl": "http://example.com/images/homemade_granola.jpg",
                  "brand": "MyKitchen",
                  "categories": [
                    "Breakfast",
                    "Snacks"
                  ],
                  "dietaryTags": [
                    "Vegetarian",
                    "Gluten-Free"
                  ]
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