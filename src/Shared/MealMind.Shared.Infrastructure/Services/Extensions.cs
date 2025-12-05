using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Infrastructure.Services;

internal static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISender, Sender>();
            services.AddScoped<IPublisher, Publisher>();
            return services;
        }
    }
}