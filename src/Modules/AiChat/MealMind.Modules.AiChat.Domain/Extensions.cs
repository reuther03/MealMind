using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.AiChat.Domain;

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