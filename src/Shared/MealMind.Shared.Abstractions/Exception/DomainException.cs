namespace MealMind.Shared.Abstractions.Exception;

public class DomainException : System.Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string messageFormat, params object[] args)
        : base(string.Format(messageFormat, args.Select(x => x.ToString())))
    {
    }
}