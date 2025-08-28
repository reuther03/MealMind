using System.Text.RegularExpressions;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects;

public sealed partial record Isbn : ValueObject
{
    public string Value { get; set; }

    public Isbn(string value)
    {
        if (!IsValid(value))
            throw new DomainException("ISBN is not valid", nameof(value));

        Value = value.ToLowerInvariant();
    }

    public static bool IsValid(string isbn) => IsbnRegex().IsMatch(isbn);

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^(?:ISBN(?:-13)?:?\s*)?97[89](?:[- ]?\d){10}$")]
    private static partial Regex IsbnRegex();
}