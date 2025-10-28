using MealMind.Modules.Nutrition.Domain.UserProfile;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.Nutrition.Infrastructure.Database.Configurations;

public class WeightHistoryConfiguration : IEntityTypeConfiguration<WeightHistory>
{
    public void Configure(EntityTypeBuilder<WeightHistory> builder)
    {
        builder.ToTable("WeightHistory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.UserProfileId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(wh => wh.Weight)
            .IsRequired()
            .HasColumnType("decimal(5,2)");
    }
}