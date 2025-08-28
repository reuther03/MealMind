using System.Text.RegularExpressions;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects;

public sealed partial record Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        if (!IsValid(value))
            throw new DomainException("Email is not valid");
        Value = value.ToLowerInvariant();
    }

    public static bool IsValid(string email) => EmailRegex().IsMatch(email);

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string email) => new(email);


    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-z]{2,}$")]
    private static partial Regex EmailRegex();
}