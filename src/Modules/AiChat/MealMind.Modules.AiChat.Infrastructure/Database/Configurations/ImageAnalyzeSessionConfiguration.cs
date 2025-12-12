using MealMind.Modules.AiChat.Domain.ImageAnalyze;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Configurations;

public class ImageAnalyzeSessionConfiguration : IEntityTypeConfiguration<ImageAnalyzeSession>
{
    public void Configure(EntityTypeBuilder<ImageAnalyzeSession> builder)
    {
        builder.ToTable("ImageAnalyzeSessions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => ImageAnalyzeSessionId.From(x))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => UserId.From(x))
            .IsRequired();

        builder.HasOne(x => x.ImageAnalyze)
            .WithOne()
            .HasForeignKey<ImageAnalyzeSession>("ImageAnalyzeId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Corrections)
            .WithOne()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}