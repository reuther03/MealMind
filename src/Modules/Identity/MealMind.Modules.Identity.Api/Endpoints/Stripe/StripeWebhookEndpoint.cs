using MealMind.Modules.Identity.Application.Features.Commands.CreateCheckoutSessionCommand;
using MealMind.Modules.Identity.Application.Features.Commands.UpdateSubscriptionTierCommand;
using MealMind.Modules.Identity.Application.Options;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace MealMind.Modules.Identity.Api.Endpoints.Stripe;

public class StripeWebhookEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("webhook/stripe",
                async (HttpRequest httpRequest, ISender sender, IOptions<StripeOptions> options, CancellationToken cancellationToken) =>
                {
                    try
                    {
                        var stripeOptions = options.Value;

                        using var reader = new StreamReader(httpRequest.Body);
                        var json = await reader.ReadToEndAsync(cancellationToken);

                        var stripeEvent = EventUtility.ConstructEvent(json, httpRequest.Headers["Stripe-Signature"], stripeOptions.WebhookSecret);

                        if (stripeEvent.Type != EventTypes.CheckoutSessionCompleted)
                            return Result.Ok();

                        var session = stripeEvent.Data.Object as Session;

                        if (session?.PaymentStatus != "paid")
                            return Result.Ok();

                        var userId = Guid.Parse(session.Metadata["userId"]);
                        var tier = Enum.Parse<SubscriptionTier>(session.Metadata["subscriptionTier"]);

                        var command = new UpdateSubscriptionTierCommand(userId, tier);
                        var result = await sender.Send(command, cancellationToken);

                        return result.IsSuccess
                            ? Result.Ok()
                            : Result.BadRequest(result.Message!);
                    }
                    catch (Exception ex)
                    {
                        return Result.BadRequest(ex.Message);
                    }
                })
            .AllowAnonymous()
            .WithDocumentation("Stripe Webhook Endpoint", "Handles Stripe webhook events for payment processing.",
                requestExample: "{}",
                """
                {
                  "isSuccess": false,
                  "statusCode": 400,
                  "message": "Object reference not set to an instance of an object."s
                }
                """
            );
    }
}