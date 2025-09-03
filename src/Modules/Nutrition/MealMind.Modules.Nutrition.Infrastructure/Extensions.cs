using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Infrastructure.Database;
using MealMind.Modules.Nutrition.Infrastructure.Database.Repositories;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Nutrition.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddPostgres<NutritionDbContext>()
            .AddScoped<INutritionDbContext, NutritionDbContext>()
            .AddScoped<IUserProfileRepository, UserProfileRepository>()
            .AddUnitOfWork<IUnitOfWork, UnitOfWork>();

        return services;
    }
}