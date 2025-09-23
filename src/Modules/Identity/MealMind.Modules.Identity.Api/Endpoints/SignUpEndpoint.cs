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
        endpointRouteBuilder.MapPost("sign-up",
                async (SignUpCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .AllowAnonymous()
            .WithDocumentation("Sign Up",
                "Creates a new user account with credentials and personal data. Returns the new user's ID. Gender: 0=Male, 1=Female. ActivityLevel: 1=Sedentary, 2=Light, 3=Moderate, 4=Active, 5=VeryActive.",
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
                  }
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