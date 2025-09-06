using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MealMind.Shared.Abstractions.Api;

public static class EndpointBaseExtensions
{
    public static RouteHandlerBuilder WithDocumentation(
        this RouteHandlerBuilder builder,
        string name,
        string description,
        [StringSyntax(StringSyntaxAttribute.Json)]
        string requestExample,
        [StringSyntax(StringSyntaxAttribute.Json)]
        string responseExample
    )
        => builder
            .WithName(name) // Sets the endpoint name displayed in Swagger
            .WithSummary(description) // Sets the summary shown at the top of the endpoint
            .WithDescription(CreateDescription(name, description, requestExample, responseExample)) // Sets the description (using summary for consistency)
            .WithOpenApi();


    private static string CreateDescription(string name, string description, string requestExample, string responseExample)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"**{name}**");
        sb.AppendLine();
        sb.AppendLine($"**{description}**");


        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("**Request Example:**");
        sb.AppendLine("```json");
        sb.AppendLine(requestExample);
        sb.AppendLine("```");


        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("**Response Example:**");
        sb.AppendLine("```json");
        sb.AppendLine(responseExample);
        sb.AppendLine("```");


        return sb.ToString();
    }
}