using MealMind.Modules.Nutrition.Application.Features.Commands.AddFavoriteFoodCommand;
using MealMind.Modules.Nutrition.Application.Features.Commands.AddFavoriteMealCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Post;

public class AddFavoriteMealEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/add-favorite-meal",
                async (AddFavoriteMealCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation(
                "Add Favorite Meal",
                "Adds a meal to the user's list of favorite meals.",
                """
                {
                    "mealId": "1a2b3c4d-5e6f-7g8h-9i0j-k1l2m3n4o5p6"
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