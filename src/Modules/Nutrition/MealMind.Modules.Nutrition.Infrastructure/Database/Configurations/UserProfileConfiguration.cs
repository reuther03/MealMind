using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfile");

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

        builder.OwnsOne(x => x.PersonalData, pd =>
        {
            pd.ToTable("UserProfile_PersonalData");

            pd.WithOwner()
                .HasForeignKey("UserProfileId");

            pd.HasKey("UserProfileId");

            pd.Property(x => x.Gender)
                .HasColumnName("Gender")
                .HasConversion<string>()
                .IsRequired();

            pd.Property(x => x.DateOfBirth)
                .HasColumnName("DateOfBirth")
                .IsRequired();

            pd.Property(x => x.Weight)
                .HasColumnName("Weight")
                .HasPrecision(6, 2)
                .IsRequired();

            pd.Property(x => x.Height)
                .HasColumnName("Height")
                .HasPrecision(5, 2)
                .IsRequired();

            pd.Property(x => x.WeightTarget)
                .HasColumnName("WeightTarget")
                .HasPrecision(6, 2)
                .IsRequired();

            pd.Property(x => x.ActivityLevel)
                .HasColumnName("ActivityLevel")
                .HasConversion<string>()
                .IsRequired();
        });

        builder.HasMany(x => x.NutritionTargets)
            .WithOne()
            .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}