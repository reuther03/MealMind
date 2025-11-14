using MealMind.Shared.Abstractions.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MealMind.Modules.Identity.Api.Endpoints.Stripe;

public class StripeCancelEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var assembly = typeof(StripeCancelEndpoint).Assembly;
        endpointRouteBuilder.MapGet("payment-cancel", () =>
                "Payment canceled. You can try again or choose a different plan.")
            .WithDocumentation(
                name: "Stripe Payment Cancel",
                description: "Handles payment cancellation from Stripe.",
                requestExample: "{}",
                responseExample: "\"Payment canceled. You can try again or choose a different plan.\""
            );
    }
}