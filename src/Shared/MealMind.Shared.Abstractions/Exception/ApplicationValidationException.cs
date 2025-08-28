namespace MealMind.Shared.Abstractions.Exception;

public sealed class ApplicationValidationException : System.Exception
{
    public ApplicationValidationException(string message)
        : base(message)
    {
    }

    public ApplicationValidationException(string messageFormat, params object[] args)
        : base(string.Format(messageFormat, args))
    {
    }

}