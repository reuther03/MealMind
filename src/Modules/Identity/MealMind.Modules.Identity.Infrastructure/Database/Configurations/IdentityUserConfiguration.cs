using MealMind.Modules.Identity.Domain.IdentityUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Identity.Infrastructure.Database.Configurations;

public class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        builder.ToTable("IdentityUsers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .ValueGeneratedNever();

        builder.Property(x => x.Username)
            .HasConversion(x => x.Value, x => new Name(x))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => new Email(x))
            .IsRequired();

        builder.Property(x => x.Password)
            .HasConversion(x => x.Value, x => new Password(x))
            .IsRequired();

        builder.OwnsOne(x => x.Subscription, s =>
        {
            s.ToTable("Subscription");

            s.WithOwner()
                .HasForeignKey("IdentityUserId");

            s.HasKey("IdentityUserId");

            s.Property(x => x.Tier)
                .HasConversion<string>()
                .IsRequired();

            s.Property(x => x.StripeCustomerId)
                .HasMaxLength(300);

            s.Property(x => x.StripeSubscriptionId)
                .HasMaxLength(300);

            s.Property(x => x.SubscriptionStartedAt);
            s.Property(x => x.CurrentPeriodStart);
            s.Property(x => x.CurrentPeriodEnd);
            s.Property(x => x.CanceledAt);
            s.Property(x => x.SubscriptionStatus);
        });

        builder.HasIndex(x => x.Email).IsUnique();
    }
}