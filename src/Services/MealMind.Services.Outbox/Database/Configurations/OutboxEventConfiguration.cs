using MealMind.Services.Outbox.Database.JsonConverters;
using MealMind.Services.Outbox.OutboxEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MealMind.Services.Outbox.Database.Configurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Include,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        };
        settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
        settings.Converters.Add(new UserIdGuidJsonConverter());
        settings.Converters.Add(new NameJsonConverter());
        settings.Converters.Add(new EmailJsonConverter());

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonConvert.SerializeObject(v, settings),
                v => JsonConvert.DeserializeObject<object>(v, settings)!);

        builder.Property(x => x.State)
            .HasConversion<string>();

        builder.Property(x => x.CreatedOn).IsRequired();
        builder.Property(x => x.ProcessedOn);
    }
}