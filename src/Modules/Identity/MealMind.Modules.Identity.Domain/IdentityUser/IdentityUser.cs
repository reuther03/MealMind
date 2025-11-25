using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Identity.Domain.IdentityUser;

public class IdentityUser : AggregateRoot<UserId>
{
    public Name Username { get; private set; }
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public Subscription Subscription { get; private set; }

    private IdentityUser()
    {
    }

    private IdentityUser(UserId id, Name username, Email email, Password password, Subscription subscription) : base(id)
    {
        Username = username;
        Email = email;
        Password = password;
        Subscription = subscription;
    }

    public static IdentityUser Create(Name username, Email email, Password password)
        => new(Guid.NewGuid(), username, email, password, Subscription.CreateFreeTier());

    public void UpdateSubscription(Subscription subscription)
        => Subscription = subscription;
}