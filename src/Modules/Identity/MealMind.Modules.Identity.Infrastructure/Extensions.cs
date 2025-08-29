using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Infrastructure.Database;
using MealMind.Modules.Identity.Infrastructure.Database.Repositories;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Identity.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddPostgres<IdentityDbContext>()
            .AddScoped<IIdentityDbContext, IdentityDbContext>()
            .AddScoped<IIdentityUserRepository, IdentityUserRepository>()
            .AddUnitOfWork<IUnitOfWork, UnitOfWork>();

        return services;
    }
}