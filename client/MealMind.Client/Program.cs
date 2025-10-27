using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MealMind.Client;
using MealMind.Client.Application.State;
using MealMind.Client.Infrastructure;
using MealMind.Client.Infrastructure.Abstractions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var services = builder.Services;


services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000/")
});

#region Extensions

// Register Blazored LocalStorage
services.AddBlazoredLocalStorage();

// Register Application State
services.AddScoped<AuthState>();

// Register Infrastructure Services
services.AddScoped<IApiClient, ApiClient>();

#endregion

var app = builder.Build();

// Initialize AuthState from localStorage
var authState = app.Services.GetRequiredService<AuthState>();
await authState.InitializeAsync();

await app.RunAsync();