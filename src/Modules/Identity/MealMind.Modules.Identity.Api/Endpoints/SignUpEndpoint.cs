using MealMind.Modules.Identity.Application.Features.Commands.SignUpCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints;

internal sealed class SignUpEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/sign-up",
                async (SignUpCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .AllowAnonymous()
            .WithDocumentation("Sign Up",
                "Creates a new user account with credentials, personal data, and nutrition targets. Pre-creates 90 days of daily logs. All days of the week must be covered by nutrition targets. Gender: 0=Male, 1=Female. ActivityLevel: 1=Sedentary, 2=Light, 3=Moderate, 4=Active, 5=VeryActive.",
                """
                {
                  "username": "johndoe",
                  "email": "john.doe@example.com",
                  "inputPassword": "SecurePassword123!",
                  "personalData": {
                    "gender": 0,
                    "dateOfBirth": "1990-01-15",
                    "weight": 75.5,
                    "height": 180.0,
                    "weightTarget": 70.0,
                    "activityLevel": 3
                  },
                  "nutritionTargets": [
                    {
                      "calories": 2500,
                      "nutritionInGramsPayload": {
                        "proteinInGrams": 180,
                        "carbohydratesInGrams": 250,
                        "fatsInGrams": 70
                      },
                      "nutritionInPercentPayload": null,
                      "waterIntake": 3.0,
                      "activeDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
                    },
                    {
                      "calories": 2000,
                      "nutritionInGramsPayload": {
                        "proteinInGrams": 150,
                        "carbohydratesInGrams": 200,
                        "fatsInGrams": 55
                      },
                      "nutritionInPercentPayload": null,
                      "waterIntake": 2.5,
                      "activeDays": ["Saturday", "Sunday"]
                    }
                  ]
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