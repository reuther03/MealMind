using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}