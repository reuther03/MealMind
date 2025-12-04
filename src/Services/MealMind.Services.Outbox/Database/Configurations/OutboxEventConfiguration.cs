using MealMind.Services.Outbox.OutboxEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MealMind.Services.Outbox.Database.Configurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OutboxEvent> builder)
    {
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
            NullValueHandling = NullValueHandling.Include
        };
        settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonConvert.SerializeObject(v, settings),
                v => JsonConvert.DeserializeObject<object>(v, settings)!);

        builder.Property(x => x.State);
        builder.Property(x => x.CreatedOn).IsRequired();
        builder.Property(x => x.ProcessedOn);
    }
}