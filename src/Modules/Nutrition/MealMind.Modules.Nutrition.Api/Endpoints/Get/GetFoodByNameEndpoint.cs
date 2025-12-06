using MealMind.Modules.Nutrition.Application.Features.Queries;
using MealMind.Modules.Nutrition.Application.Features.Queries.GetFoodByNameQuery;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints.Get;

public class GetFoodByNameEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("food/search",
                async ([AsParameters] GetFoodByNameRequest request, ISender sender) =>
                {
                    var result = await sender.Send(new GetFoodByNameQuery(request.SearchTerm, request.PageSize, request.Page));
                    return result;
                })
            .AllowAnonymous()
            .WithDocumentation(
                "Search Food by Name",
                "Searches for food items by name with pagination. Returns nutritional information including calories, macronutrients (protein, carbs, fats), and micronutrients. Data is sourced from both the local database and OpenFoodFacts API.",
                "GET http://localhost:5000/food/search?SearchTerm=123&PageSize=10&Page=1",
                """
                {
                  "value": {
                    "items": [
                      {
                        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "name": "Chicken Breast",
                        "calories": 165,
                        "protein": 31.0,
                        "carbohydrates": 0.0,
                        "fats": 3.6,
                        "fiber": 0.0,
                        "sugar": 0.0,
                        "sodium": 74.0
                      }
                    ],
                    "page": 1,
                    "pageSize": 10,
                    "totalCount": 1,
                    "hasNextPage": false,
                    "hasPreviousPage": false
                  },
                  "isSuccess": true,
                  "statusCode": 200,
                  "message": null
                }
                """
            );
    }
}

public record GetFoodByNameRequest(string SearchTerm, int PageSize = 10, int Page = 1);