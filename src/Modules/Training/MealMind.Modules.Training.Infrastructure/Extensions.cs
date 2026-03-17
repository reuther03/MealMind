using MealMind.Modules.Training.Application.Abstractions.Database;
using MealMind.Modules.Training.Infrastructure.Database;
using MealMind.Modules.Training.Infrastructure.Database.Repositories;
using MealMind.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Modules.Training.Infrastructure;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddPostgres<TrainingDbContext>(configuration)
                .AddScoped<ITrainingDbContext, TrainingDbContext>()
                .AddScoped<ITrainingPlanRepository, TrainingPlanRepository>()
                .AddUnitOfWork<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}