using MealMind.Shared.Abstractions.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints.Stripe;

public class StripeSuccessPaymentEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("payment-success", () =>
            "Payment successful! Check your account.");
    }
}