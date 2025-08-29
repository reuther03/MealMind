using MealMind.Bootstrapper;
using MealMind.Shared.Infrastructure;
using MealMind.Shared.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
var services = builder.Services;
var configuration = builder.Configuration;

builder.ConfigureModules();

services.AddEndpointsApiExplorer();

var assemblies = ModuleLoader.LoadAssemblies(services, configuration);
var modules = ModuleLoader.LoadModules(assemblies);

services.AddInfrastructure(assemblies, modules, configuration);

foreach (var module in modules)
{
    module.Register(services);
}

var app = builder.Build();

app.UseInfrastructure();
foreach (var module in modules)
{
    module.Use(app);
}

await app.RunAsync();