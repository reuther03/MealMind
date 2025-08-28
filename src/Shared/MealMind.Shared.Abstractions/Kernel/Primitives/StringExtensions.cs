namespace MealMind.Shared.Abstractions.Kernel.Primitives;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input)
    {
        switch (input)
        {
            case null:
                throw new ArgumentNullException(nameof(input));
            case "":
                throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default:
                input = input.ToLowerInvariant();

                return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
        }
    }
}