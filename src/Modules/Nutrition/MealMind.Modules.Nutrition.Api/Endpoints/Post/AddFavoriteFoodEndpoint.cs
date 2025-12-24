using MealMind.Modules.Nutrition.Application.Features.Commands.AddCustomFoodCommand;
using MealMind.Modules.Nutrition.Application.Features.Commands.AddFavoriteFoodCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Post;

public class AddFavoriteFoodEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/add-favorite-food",
                async (AddFavoriteFoodCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Add Favorite Food",
                "Adds a food item to the user's list of favorite foods.",
                """
                {
                  "userId": "8b3c7d92-4f6e-4a1c-9e2b-5f8a3c6d1e9f",
                  "foodId": "1a2b3c4d-5e6f-7g8h-9i0j-k1l2m3n4o5p6"
                }
                """,
                """
                {
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}