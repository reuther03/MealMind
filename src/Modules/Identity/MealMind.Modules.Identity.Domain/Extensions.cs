using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Identity.Domain;

public static class Extensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}