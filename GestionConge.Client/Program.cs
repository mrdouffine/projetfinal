using GestionConge.Client;
using GestionConge.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Services front (consommation API)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CongeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
