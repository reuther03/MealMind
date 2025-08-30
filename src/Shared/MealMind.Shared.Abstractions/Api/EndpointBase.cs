using Microsoft.AspNetCore.Routing;

namespace Stoxly.Shared.Abstractions.Api;

public abstract class EndpointBase
{
    public abstract void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder);
}