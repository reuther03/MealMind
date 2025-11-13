using MealMind.Modules.Identity.Application.Abstractions;
using MealMind.Modules.Identity.Application.Abstractions.Database;
using MealMind.Modules.Identity.Application.Abstractions.Services;
using MealMind.Modules.Identity.Application.Options;
using MealMind.Modules.Identity.Infrastructure.Database;
using MealMind.Modules.Identity.Infrastructure.Database.Repositories;
using MealMind.Modules.Identity.Infrastructure.Database.Services;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Identity.Infrastructure;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            services.Configure<StripeOptions>(configuration.GetSection("stripe"));

            services
                .AddPostgres<IdentityDbContext>()
                .AddScoped<IIdentityDbContext, IdentityDbContext>()
                .AddScoped<IIdentityUserRepository, IdentityUserRepository>()
                .AddScoped<IStripeService, StripeService>()
                .AddUnitOfWork<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}