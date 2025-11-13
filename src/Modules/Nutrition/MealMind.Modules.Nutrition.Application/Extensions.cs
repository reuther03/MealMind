using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Nutrition.Application;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            return services;
        }
    }
}