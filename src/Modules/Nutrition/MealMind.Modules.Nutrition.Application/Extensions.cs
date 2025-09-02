using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Nutrition.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}