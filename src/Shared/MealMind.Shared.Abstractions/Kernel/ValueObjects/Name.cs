using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects;

public sealed record Name : ValueObject
{
    public string Value { get; set; }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 100 or < 3)
        {
            throw new ArgumentException("Name cannot be empty", nameof(value));
        }

        Value = value.FirstCharToUpper();
    }

    public static implicit operator Name(string value) => new(value);
    public static implicit operator string(Name fullname) => fullname.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}