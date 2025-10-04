using MealMind.Modules.Nutrition.Application.Abstractions;
using MealMind.Modules.Nutrition.Application.Abstractions.Database;
using MealMind.Modules.Nutrition.Application.Abstractions.Services;
using MealMind.Modules.Nutrition.Infrastructure.Database;
using MealMind.Modules.Nutrition.Infrastructure.Database.Repositories;
using MealMind.Modules.Nutrition.Infrastructure.Database.Services;
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
            .AddScoped<IFoodRepository, FoodRepository>()
            .AddScoped<IOpenFoodFactsService, OpenFoodFactsService>()
            .AddUnitOfWork<IUnitOfWork, UnitOfWork>()
            .AddHttpClient<IOpenFoodFactsService, OpenFoodFactsService>(opt =>
            {
                opt.BaseAddress = new Uri("https://world.openfoodfacts.net");
                opt.DefaultRequestHeaders.Add("User-Agent", "MealMind/1.0");
                opt.Timeout = TimeSpan.FromSeconds(10);
            });

        return services;
    }
}