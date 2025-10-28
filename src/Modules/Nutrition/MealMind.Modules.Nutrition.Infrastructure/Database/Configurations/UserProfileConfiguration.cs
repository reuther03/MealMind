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
            pd.ToTable("PersonalData");

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

        builder.OwnsMany(x => x.FavoriteFoods, ownedBuilder =>
        {
            ownedBuilder.WithOwner().HasForeignKey("UserProfileId");
            ownedBuilder.ToTable("FavoriteFoods");
            ownedBuilder.HasKey("Id");

            ownedBuilder.Property(x => x.Value)
                .ValueGeneratedNever()
                .HasColumnName("FoodId");

            builder.Metadata
                .FindNavigation(nameof(UserProfile.FavoriteFoods))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.OwnsMany(x => x.FavoriteMeals, ownedBuilder =>
        {
            ownedBuilder.WithOwner().HasForeignKey("UserProfileId");
            ownedBuilder.ToTable("FavoriteMeals");
            ownedBuilder.HasKey("Id");

            ownedBuilder.Property(x => x.Value)
                .ValueGeneratedNever()
                .HasColumnName("MealId");

            builder.Metadata
                .FindNavigation(nameof(UserProfile.FavoriteMeals))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}