using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Shared.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace MealMind.Modules.AiChat.Infrastructure.Database.Seeders;

public class AiChatModuleSeeder : IModuleSeeder
{
    private readonly IAiChatDbContext _dbContext;

    public AiChatModuleSeeder(IAiChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SeedAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}