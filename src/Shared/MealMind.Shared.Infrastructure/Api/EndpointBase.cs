using Microsoft.AspNetCore.Routing;

namespace MealMind.Shared.Infrastructure.Api;

public abstract class EndpointBase
{
    public abstract void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder);
}