using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISender, Sender>();
        services.AddScoped<IPublisher, Publisher>();

        return services;
    }
}