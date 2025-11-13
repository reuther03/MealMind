using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Identity.Domain;

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