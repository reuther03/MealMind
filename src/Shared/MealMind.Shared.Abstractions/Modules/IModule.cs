using MealMind.Shared.Abstractions.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MealMind.Shared.Abstractions.Modules;

public interface IModule
{
    string Name { get; }
    string Path { get; }
    void Register(IServiceCollection services);
    void Use(WebApplication app);

    public void AddModuleEndpoints(WebApplication app)
    {
        var assembly = GetType().Assembly;
        var endpointTypes = assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(EndpointBase)) && !type.IsAbstract)
            .Select(type => ActivatorUtilities.CreateInstance(app.Services, type) as EndpointBase)
            .Where(endpoint => endpoint is not null)
            .ToList();

        endpointTypes.ForEach(endpoint => endpoint!.AddEndpoint(app));
    }
}