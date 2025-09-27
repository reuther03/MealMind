using System.Text.Json;
using System.Text.Json.Serialization;

namespace MealMind.Modules.Nutrition.Infrastructure.External.OpenFoodFacts.Converters;

public class FlexibleNumberConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? stringValue = reader.GetString();
            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }
        }

        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}