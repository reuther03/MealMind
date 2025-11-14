using MealMind.Modules.Identity.Application.Features.Commands.UpdateSubscriptionAfterPaymentCommand;
using MealMind.Modules.Identity.Application.Features.Commands.UpdateSubscriptionTierCommand;
using MealMind.Modules.Identity.Application.Features.Payloads;
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

                        switch (stripeEvent.Type)
                        {
                            case EventTypes.CheckoutSessionCompleted:
                                await EventTypeCheckoutSessionCompleted(sender, stripeEvent, cancellationToken);
                                break;
                            
                            case EventTypes.InvoicePaymentSucceeded:
                                await EventTypeInvoicePaid(sender, stripeEvent, cancellationToken);
                                break;

                            case EventTypes.CustomerSubscriptionUpdated:
                                // Handle customer subscription updated event
                                break;

                            case EventTypes.CustomerSubscriptionDeleted:
                                // Handle customer subscription deleted event
                                break;

                            default:
                                return Result.BadRequest($"Unhandled event type: {stripeEvent.Type}");
                        }
                    }
                    catch (Exception ex)
                    {
                        return Result.BadRequest(ex.Message);
                    }

                    return Result.Ok();
                })
            .AllowAnonymous()
            .WithDocumentation("Stripe Webhook Endpoint", "Handles Stripe webhook events for payment processing.",
                requestExample: "{}",
                """
                {
                  "isSuccess": false,
                  "statusCode": 400,
                  "message": "Object reference not set to an instance of an object."
                }
                """
            );
    }

    private static async Task EventTypeCheckoutSessionCompleted(ISender sender, Event stripeEvent, CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Session;

        if (session == null)
            return;

        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.GetAsync(session.SubscriptionId, cancellationToken: cancellationToken);

        if (subscription == null)
            return;

        // Fetch the invoice to get period dates - LatestInvoiceId is a string ID, not expanded object
        var invoiceService = new InvoiceService();
        var invoice = await invoiceService.GetAsync(subscription.LatestInvoiceId, cancellationToken: cancellationToken);

        if (invoice == null)
            return;

        var userId = Guid.Parse(session.Metadata["userId"]);
        var tier = Enum.Parse<SubscriptionTier>(session.Metadata["subscriptionTier"]);
        var customerId = session.CustomerId;
        var subscriptionId = subscription.Id;
        var subscriptionStartedAt = subscription.StartDate;
        var subscriptionCurrentPeriodStart = invoice.PeriodStart;
        var subscriptionCurrentPeriodEnd = invoice.PeriodEnd;
        var subscriptionStatus = subscription.Status;

        await sender.Send(
            new UpdateSubscriptionTierCommand(
                new UpdateSubscriptionTierPayload(
                    userId, tier, customerId, subscriptionId,
                    subscriptionStartedAt, subscriptionCurrentPeriodStart,
                    subscriptionCurrentPeriodEnd, subscriptionStatus)
            ), cancellationToken);
    }

    private static async Task EventTypeInvoicePaid(ISender sender, Event stripeEvent, CancellationToken cancellationToken)
    {
        var invoice = stripeEvent.Data.Object as Invoice;

        if (invoice == null)
            return;

        var subscriptionId = invoice.Lines.Data[0].SubscriptionId;

        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.GetAsync(subscriptionId, cancellationToken: cancellationToken);

        if (subscription == null)
            return;

        var tier = Enum.Parse<SubscriptionTier>(
            subscription.Metadata["subscriptionTier"]);

        await sender.Send(
            new UpdateSubscriptionAfterPaymentCommand(
                subscription.Id,
                tier,
                invoice.PeriodStart,
                invoice.PeriodEnd,
                subscription.Status),
            cancellationToken);
    }
}