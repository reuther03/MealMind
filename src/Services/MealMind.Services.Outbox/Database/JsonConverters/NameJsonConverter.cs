using MealMind.Shared.Abstractions.Kernel.ValueObjects;

namespace MealMind.Services.Outbox.Database.JsonConverters;

public class NameJsonConverter : ValueObjectConverter<Name, string>
{
    protected override string GetValue(Name obj)
        => obj.Value;

    protected override Name CreateFrom(string value)
        => new(value);
}