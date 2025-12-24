using MealMind.Modules.Identity.Application.Features.Commands.Stripe.StripeSubscriptionTierChangesCommand;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.SubscriptionDeletedCommand;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.UpdateSubscriptionAfterPaymentCommand;
using MealMind.Modules.Identity.Application.Features.Commands.Stripe.UpdateSubscriptionTierCommand;
using MealMind.Modules.Identity.Application.Features.Payloads;
using MealMind.Modules.Identity.Application.Options;
using MealMind.Shared.Abstractions.Api;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Result;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace MealMind.Modules.Identity.Api.Endpoints.Stripe;

public class StripeWebhookEndpoint : EndpointBase
{
    public override void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/webhook/stripe",
                async (HttpRequest httpRequest, ISender sender, IOptions<StripeOptions> options, ILogger<StripeWebhookEndpoint> logger,
                    CancellationToken cancellationToken) =>
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
                                await EventTypeCheckoutSessionCompleted(sender, stripeEvent, logger, cancellationToken);
                                break;

                            case EventTypes.InvoicePaymentSucceeded:
                                await EventTypeInvoicePaid(sender, stripeEvent, logger, cancellationToken);
                                break;

                            case EventTypes.CustomerSubscriptionUpdated:
                                await EventTypeCustomerSubscriptionUpdated(sender, stripeEvent, logger, cancellationToken);
                                break;

                            case EventTypes.CustomerSubscriptionDeleted:
                                await EventTypesCustomerSubscriptionDeleted(sender, stripeEvent, logger, cancellationToken);
                                break;

                            default:
                                return Result.BadRequest($"Unhandled event type: {stripeEvent.Type}");
                        }
                    }
                    catch (Exception ex)
                    {
                        return Result.Ok(ex.Message);
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

    private static async Task EventTypeCheckoutSessionCompleted(ISender sender, Event stripeEvent, ILogger<StripeWebhookEndpoint> logger,
        CancellationToken cancellationToken)
    {
        if (stripeEvent.Data.Object is not Session session)
        {
            logger.LogWarning("CheckoutSessionCompleted event received with null session object.");
            return;
        }

        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.GetAsync(session.SubscriptionId, cancellationToken: cancellationToken);

        if (subscription == null)
        {
            logger.LogWarning("CheckoutSessionCompleted event received with null subscription object.");
            return;
        }

        if (subscription.Items.Data == null || subscription.Items.Data.Count == 0)
        {
            logger.LogWarning("CheckoutSessionCompleted event received with no subscription items.");
            return;
        }

        var userId = Guid.Parse(session.Metadata["userId"]);
        var tier = Enum.Parse<SubscriptionTier>(session.Metadata["subscriptionTier"]);
        var customerId = session.CustomerId;
        var subscriptionId = subscription.Id;
        var subscriptionStartedAt = subscription.StartDate;
        var subscriptionCurrentPeriodStart = subscription.Items.Data[0].CurrentPeriodStart;
        var subscriptionCurrentPeriodEnd = subscription.Items.Data[0].CurrentPeriodEnd;
        var subscriptionStatus = subscription.Status;

        await sender.Send(
            new UpdateSubscriptionTierCommand(
                new UpdateSubscriptionTierPayload(
                    userId, tier, customerId, subscriptionId,
                    subscriptionStartedAt, subscriptionCurrentPeriodStart,
                    subscriptionCurrentPeriodEnd, subscriptionStatus)
            ), cancellationToken);
    }

    private static async Task EventTypeInvoicePaid(ISender sender, Event stripeEvent, ILogger<StripeWebhookEndpoint> logger,
        CancellationToken cancellationToken)
    {
        if (stripeEvent.Data.Object is not Invoice invoice)
        {
            logger.LogWarning("InvoicePaid event received with null invoice object.");
            return;
        }

        if (invoice.BillingReason != "subscription_cycle")
        {
            logger.LogInformation("InvoicePaid event received with billing reason not equal to 'subscription_cycle'. Ignoring.");
            return;
        }

        if (invoice.Lines?.Data == null || invoice.Lines.Data.Count == 0)
        {
            logger.LogWarning("InvoicePaid event received with no invoice lines.");
            return;
        }

        var subscriptionId = invoice.Parent.SubscriptionDetails.SubscriptionId;

        if (string.IsNullOrEmpty(subscriptionId))
        {
            logger.LogWarning("InvoicePaid event received with null subscription ID.");
            return;
        }

        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.GetAsync(invoice.Parent.SubscriptionDetails.SubscriptionId, cancellationToken: cancellationToken);

        if (subscription == null)
        {
            logger.LogWarning("InvoicePaid event received with null subscription object.");
            return;
        }

        var tier = Enum.Parse<SubscriptionTier>(
            subscription.Metadata["subscriptionTier"]);

        await sender.Send(
            new UpdateSubscriptionAfterPaymentCommand(
                subscription.Id,
                tier,
                invoice.Lines.Data[0].Period.Start,
                invoice.Lines.Data[0].Period.End,
                subscription.Status),
            cancellationToken);
    }

    private static async Task EventTypeCustomerSubscriptionUpdated(ISender sender, Event stripeEvent, ILogger<StripeWebhookEndpoint> logger,
        CancellationToken cancellationToken)
    {
        if (stripeEvent.Data.Object is not Subscription subscription)
        {
            logger.LogWarning("SubscriptionUpdated event received with null subscription object.");
            return;
        }

        if (subscription.Items == null || subscription.Items.Data.Count == 0)
        {
            logger.LogWarning("SubscriptionUpdated event received with no subscription items.");
            return;
        }

        var price = subscription.Items.Data[0].Price.UnitAmount;

        var tier = MapPriceToTier(price ?? 0);

        await sender.Send(
            new SubscriptionTierChangesCommand(
                tier,
                subscription.CustomerId,
                subscription.Id,
                subscription.Items.Data[0].CurrentPeriodStart,
                subscription.Items.Data[0].CurrentPeriodEnd,
                subscription.Status),
            cancellationToken);
    }

    private static async Task EventTypesCustomerSubscriptionDeleted(ISender sender, Event stripeEvent, ILogger<StripeWebhookEndpoint> logger,
        CancellationToken cancellationToken)
    {
        if (stripeEvent.Data.Object is not Subscription subscription)
        {
            logger.LogWarning("SubscriptionDeleted event received with null subscription object.");
            return;
        }

        await sender.Send(
            new SubscriptionDeletedCommand(subscription.CustomerId, subscription.CanceledAt, subscription.Status),
            cancellationToken);
    }

    private static SubscriptionTier MapPriceToTier(long price)
    {
        return price switch
        {
            999 => SubscriptionTier.Premium,
            399 => SubscriptionTier.Standard,
            _ => SubscriptionTier.Free
        };
    }
}