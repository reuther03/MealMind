using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Services.Outbox.Database.JsonConverters;

public class EmailJsonConverter : ValueObjectConverter<Email, string>
{
    protected override string GetValue(Email obj)
        => obj.Value;

    protected override Email CreateFrom(string value)
        => new(value);
}