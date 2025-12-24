using MealMind.Modules.Identity.Application.Features.Commands.Stripe.CreateCheckoutSessionCommand;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints.Stripe;

public class CreateCheckoutSessionEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/session-checkout",
                async (CreateCheckoutSessionCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);
                    return result;
                })
            .RequireAuthorization()
            .WithDocumentation("Create Checkout Session",
                "Creates a Stripe checkout session for the authenticated user based on the selected subscription tier. Returns a URL to the checkout session.",
                """
                {
                  "subscriptionTier": 2
                }
                """,
                """
                {
                  "value": "https://checkout.stripe.com/pay/cs_test_a1b2c3d4e5f6g7h8i9j0",
                  "isSuccessful": true,
                  "errors": []
                }
                """
            );
    }

}