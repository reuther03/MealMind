using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Nutrition.Domain;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomain()
        {
            return services;
        }
    }
}