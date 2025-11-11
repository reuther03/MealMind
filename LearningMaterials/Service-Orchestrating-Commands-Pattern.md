# Service Orchestrating Commands Pattern

This document explains the architectural pattern where a service (typically in Infrastructure layer) orchestrates business logic by sending commands to the Application layer.

---

## The Confusion

When learning clean architecture, you typically see this flow:

```
Api Layer (Endpoints)
   ↓
Application Layer (Commands/Handlers)
   ↓
Infrastructure Layer (Repositories/External Services)
```

**Expected rule:** Higher layers depend on lower layers.

But then you see code like this:

```csharp
// StripeWebhookHandler in Infrastructure layer
public class StripeWebhookHandler : IStripeWebhookHandler
{
    private readonly ISender _sender; // ⬅️ Wait, sending commands from Infrastructure?

    public async Task HandleCheckoutCompletedAsync(Session session)
    {
        var command = new ActivateSubscriptionCommand(...);
        await _sender.Send(command); // Infrastructure → Application ???
    }
}
```

**This feels backwards!** Infrastructure is calling Application layer. Is this allowed?

---

## Understanding the Layers

### Traditional Clean Architecture Rules

```
┌─────────────────┐
│   Api Layer     │  (Endpoints, Controllers)
└────────┬────────┘
         │ depends on ↓
┌────────▼────────┐
│ Application     │  (Commands, Queries, Handlers)
└────────┬────────┘
         │ depends on ↓
┌────────▼────────┐
│   Domain        │  (Entities, Value Objects)
└────────┬────────┘
         │ depends on ↓
┌────────▼────────┐
│ Infrastructure  │  (Repositories, External APIs)
└─────────────────┘
```

**Dependency Rule:**
- ✅ Api → Application (Allowed)
- ✅ Application → Domain (Allowed)
- ✅ Application → Infrastructure **via interfaces** (Allowed through DI)
- ❌ Infrastructure → Application (Wait... is this allowed?)

---

## The Answer: Yes, It's Allowed!

### Why Infrastructure Can Call Application

**Key insight:** The dependency flow is about **abstractions**, not concrete implementations.

```
Infrastructure → ISender (abstraction) ← Application implements handlers
```

**Detailed flow:**

1. Infrastructure imports `ISender` interface (from Application or Shared)
2. Infrastructure calls `_sender.Send(command)`
3. At runtime, DI resolves the command handler (in Application layer)
4. Handler executes business logic

**Dependency direction:**
- Infrastructure depends on **abstraction** (`ISender`)
- Infrastructure does **NOT** depend on concrete handler implementations
- Application layer is still **independent** and **testable**

### Dependency Inversion Principle

This follows the **Dependency Inversion Principle** (the "D" in SOLID):

> High-level modules should not depend on low-level modules. Both should depend on abstractions.

```
┌─────────────────┐
│ Infrastructure  │ ──depends on──> ISender (abstraction)
└─────────────────┘                     ▲
                                        │
                                        │ implements
                                        │
                               ┌────────┴────────┐
                               │  Application    │
                               │  (Handlers)     │
                               └─────────────────┘
```

**At runtime:**
- Infrastructure calls `ISender.Send()`
- DI container resolves the appropriate handler from Application layer
- Handler executes

**At compile time:**
- Infrastructure only knows about `ISender` interface
- Infrastructure has **no direct reference** to Application layer
- Clean separation maintained

---

## When Does This Pattern Appear?

### Common Scenario: Webhook Handlers

Webhooks are a perfect example:

```csharp
// Infrastructure layer - deals with external system (Stripe)
public class StripeWebhookHandler : IStripeWebhookHandler
{
    private readonly ISender _sender;
    private readonly IIdentityUserRepository _repository;

    public async Task HandleSubscriptionDeletedAsync(Subscription subscription)
    {
        // 1. Infrastructure concern: Parse Stripe data
        var subscriptionId = subscription.Id;

        // 2. Infrastructure concern: Find user by external identifier
        var user = await _repository.GetByStripeSubscriptionIdAsync(subscriptionId);

        if (user == null) return;

        // 3. Application concern: Business logic via command
        var command = new CancelSubscriptionCommand(user.UserId);
        await _sender.Send(command);
    }
}
```

**Why this pattern?**
- Webhook handling is **infrastructure** (dealing with external Stripe API)
- But updating subscription is **business logic** (belongs in Application)
- Service **coordinates** between infrastructure and application

---

## Alternative Architectures

### Option 1: Service in Infrastructure (What We Discussed)

```
Endpoint (Api)
   ↓
StripeWebhookHandler (Infrastructure)
   ↓
Command Handler (Application)
   ↓
Repository (Infrastructure)
```

**Pros:**
- Stripe-specific logic lives in Infrastructure
- Endpoint stays thin
- Clear separation: parsing webhooks vs business logic

**Cons:**
- Infrastructure → Application feels "backwards"
- Can be conceptually confusing

### Option 2: Service in Application Layer

Move the webhook handler to Application layer:

```
src/Modules/Identity/MealMind.Modules.Identity.Application/
└── Services/
    └── StripeWebhookHandler.cs
```

```
Endpoint (Api)
   ↓
StripeWebhookHandler (Application)
   ↓
Commands (Application)
   ↓
Repository (Infrastructure)
```

**Pros:**
- "Feels" more correct architecturally
- Clear flow: Api → Application → Infrastructure
- Orchestration is business logic (belongs in Application)

**Cons:**
- Application layer now knows about Stripe events (coupling to external system)
- Stripe.NET types leak into Application layer

**Solution to cons:** Use DTOs to wrap Stripe types:

```csharp
// Application layer
public interface IStripeWebhookHandler
{
    Task HandleCheckoutCompletedAsync(CheckoutSessionDto session);
}

public record CheckoutSessionDto(
    string SubscriptionId,
    string CustomerId,
    Dictionary<string, string> Metadata
);

// Infrastructure creates DTOs from Stripe types
var dto = new CheckoutSessionDto(
    session.SubscriptionId,
    session.CustomerId,
    session.Metadata
);
await _webhookHandler.HandleCheckoutCompletedAsync(dto);
```

### Option 3: No Service - Commands Only (Simplest)

Don't create a webhook handler service at all:

```csharp
// Endpoint directly creates commands
endpointRouteBuilder.MapPost("webhook/stripe", async (
    HttpRequest request,
    ISender sender) =>
{
    var stripeEvent = EventUtility.ConstructEvent(...);

    switch (stripeEvent.Type)
    {
        case Events.CheckoutSessionCompleted:
            var session = stripeEvent.Data.Object as Session;

            var command = new ActivateSubscriptionCommand(
                Guid.Parse(session.Metadata["userId"]),
                session.SubscriptionId,
                session.CustomerId
            );

            await sender.Send(command);
            break;
    }
});
```

**Pros:**
- ✅ Simplest - no extra service layer
- ✅ Clear flow: Api → Application
- ✅ Pure CQRS

**Cons:**
- Endpoint gets larger with multiple webhook types
- Stripe-specific parsing logic in Api layer

---

## Which Approach to Use?

### For Learning: Option 3 (No Service)

**Recommended for beginners:**
- Clearest architecture
- Follows standard CQRS pattern
- No confusion about layering

```csharp
// Endpoint
switch (stripeEvent.Type)
{
    case Events.CheckoutSessionCompleted:
        var command = new ActivateSubscriptionCommand(...);
        await sender.Send(command);
        break;
}

// Command Handler (Application)
public async Task<Result> Handle(ActivateSubscriptionCommand request)
{
    var user = await _repository.GetByIdAsync(request.UserId);
    user.UpdateSubscriptionTier(...);
    await _unitOfWork.CommitAsync();
}
```

### For Production: Option 1 or 2

**When webhook logic becomes complex:**

**Option 1 (Service in Infrastructure):**
- Use when dealing heavily with external system specifics
- Example: Complex Stripe event parsing, retries, error handling

**Option 2 (Service in Application):**
- Use when orchestration logic is complex business logic
- Example: Multiple commands per webhook, conditional logic based on user state

---

## Key Takeaways

1. **Infrastructure → Application via `ISender` is allowed**
   - Follows Dependency Inversion Principle
   - Infrastructure depends on abstraction, not concrete implementations

2. **Service orchestrating commands is a coordination pattern**
   - Not business logic itself
   - Delegates to commands for actual business logic

3. **Where to put the service depends on its primary concern**
   - Heavy external API logic → Infrastructure
   - Heavy business logic → Application
   - Simple webhook handling → No service needed

4. **For webhooks specifically:**
   - Simple approach: Endpoint → Command → Handler
   - Complex approach: Endpoint → Service → Commands → Handlers

5. **The "backwards" feeling is normal**
   - Clean architecture is about abstractions, not physical layer order
   - Runtime execution can flow in different directions
   - As long as abstractions are respected, it's correct

---

## Example: Complete Flow

### Setup

```csharp
// Shared.Abstractions
public interface ISender
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command);
}

// Infrastructure
public class StripeWebhookHandler : IStripeWebhookHandler
{
    private readonly ISender _sender; // Depends on abstraction

    public async Task HandleCheckoutCompletedAsync(Session session)
    {
        var command = new ActivateSubscriptionCommand(
            Guid.Parse(session.Metadata["userId"]),
            session.SubscriptionId
        );

        await _sender.Send(command); // Calls through abstraction
    }
}

// Application
public record ActivateSubscriptionCommand(
    Guid UserId,
    string StripeSubscriptionId
) : ICommand<Result>;

internal sealed class Handler : ICommandHandler<ActivateSubscriptionCommand, Result>
{
    public async Task<Result> Handle(ActivateSubscriptionCommand request)
    {
        // Business logic here
    }
}

// DI Registration
services.AddScoped<ISender, Sender>(); // Runtime binds abstraction to implementation
services.AddScoped<IStripeWebhookHandler, StripeWebhookHandler>();
```

### Runtime Flow

```
1. Stripe sends webhook → POST /webhook/stripe

2. Endpoint receives request
   ↓
3. Endpoint calls: webhookHandler.HandleCheckoutCompletedAsync(session)
   ↓
4. StripeWebhookHandler (Infrastructure) calls: _sender.Send(command)
   ↓
5. Sender (shared infrastructure) finds handler via DI
   ↓
6. Handler (Application) executes business logic
   ↓
7. Handler calls repository (Infrastructure via interface)
   ↓
8. Database updated
```

**Dependency direction at compile time:**
```
Api → Application abstractions
Infrastructure → Application abstractions (ISender)
Application → Domain
Application → Infrastructure abstractions (IRepository)
```

**All dependencies point inward (toward Domain), which is correct!**

---

## Conclusion

The "service sending commands" pattern **is valid** and follows clean architecture principles when done through abstractions like `ISender`.

**For your Stripe integration:**
- Start with **no service** (Option 3) - simplest and clearest
- Add service layer later if webhook logic becomes complex
- Either way, keep business logic in command handlers

The key is understanding that **runtime flow** and **compile-time dependencies** can differ, and that's okay as long as you respect the Dependency Inversion Principle.
