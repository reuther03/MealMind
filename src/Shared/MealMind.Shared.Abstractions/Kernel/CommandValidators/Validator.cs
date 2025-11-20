using System.Diagnostics.CodeAnalysis;

namespace MealMind.Shared.Abstractions.Kernel.CommandValidators;

public static class Validator
{
    public static void ValidateNotNull<T>([NotNull] T value)
    {
        if (value is null)
            throw new ArgumentNullException($"{typeof(T).Name} not found");
    }
}