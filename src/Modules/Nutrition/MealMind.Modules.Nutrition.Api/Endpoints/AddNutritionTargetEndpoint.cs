using MealMind.Modules.Nutrition.Application.Features.Commands.AddNutritionTargetCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Kernel.Primitives.Result;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Nutrition.Api.Endpoints;

public class AddNutritionTargetEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("nutrition-target",
                async (AddNutritionTargetCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Add Nutrition Target",
                "Creates a nutrition target for the authenticated user. Provide either nutritionInGramsPayload OR nutritionInPercentPayload (not both). ActiveDays: 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday.",
                """
                {
                  "calories": 2000,
                  "nutritionInGramsPayload": {
                    "proteinInGrams": 150,
                    "carbohydratesInGrams": 200,
                    "fatsInGrams": 65
                  },
                  "nutritionInPercentPayload": null,
                  "waterIntake": 3.5,
                  "activeDays": [1, 2, 3, 4, 5]
                }
                """,
                """
                {
                  "value": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                  "isSuccessful": true,
                  "errors": []
                }
                """
            );
    }
}