using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Identity.Application;

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