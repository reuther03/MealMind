namespace MealMind.Shared.Abstractions.QueriesAndCommands.Requests;

public interface IRequestBase
{
    // Marker interface for all request types
}

public interface IRequest<out TResponse> : IRequestBase
{
    // Marker interface for all request types with a response
}