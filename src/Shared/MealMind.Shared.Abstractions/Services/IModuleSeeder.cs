using Microsoft.Extensions.Configuration;

namespace MealMind.Shared.Abstractions.Services;

public interface IModuleSeeder
{
    Task SeedAsync(IConfiguration configuration, CancellationToken cancellationToken);
}