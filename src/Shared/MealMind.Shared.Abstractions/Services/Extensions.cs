using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Abstractions.Services;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices()
        {
            return services;
        }
    }
}