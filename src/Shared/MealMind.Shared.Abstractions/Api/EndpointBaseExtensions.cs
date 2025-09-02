using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MealMind.Shared.Abstractions.Api;

public static class EndpointBaseExtensions
{
    public static RouteHandlerBuilder WithDocumentation(
        this RouteHandlerBuilder builder,
        string name,
        string summary,
        string description
    )
        => builder
            .WithName(name)
            .WithSummary(summary)
            .WithDescription(description)
            .WithOpenApi();
}