using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GestionConge.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
// Remplace BaseAddress par l'URL de ton back-end API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7064/") });
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ApiClient>();

builder.Services.AddScoped<DemandeService>();
builder.Services.AddScoped<UserService>();



// Restaure la session depuis localStorage avant d'afficher l'app
//var auth = builder.Build().Services.GetRequiredService<AuthService>();
//await auth.TryRestoreAsync();

//var authServices = new AuthServices(builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>());


await builder.Build().RunAsync();


