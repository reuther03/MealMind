using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using Newtonsoft.Json;

namespace MealMind.Services.Outbox.Database.JsonConverters;

public abstract class ValueObjectConverter<TValueObject, TPrimitive> : JsonConverter<TValueObject>
{
    protected abstract TPrimitive GetValue(TValueObject obj);
    protected abstract TValueObject CreateFrom(TPrimitive value);

    public override void WriteJson(JsonWriter writer, TValueObject? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var primitive = GetValue(value);
        writer.WriteValue(primitive);
    }

    public override TValueObject ReadJson(JsonReader reader, Type objectType, TValueObject? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return default!;

        var primitive = serializer.Deserialize<TPrimitive>(reader);
        return CreateFrom(primitive!);
    }
}