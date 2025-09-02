using Microsoft.AspNetCore.Routing;

namespace MealMind.Shared.Abstractions.Api;

public abstract class EndpointBase
{
    public abstract void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder);
}