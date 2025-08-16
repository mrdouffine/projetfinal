using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GestionConge.Client.Services;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
// Remplace BaseAddress par l'URL de ton back-end API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7064/") });
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ApiClient>();

builder.Services.AddScoped<DemandeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthServices>();


var authServices = new AuthServices(builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>());


await builder.Build().RunAsync();


