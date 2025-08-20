using GestionConge.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Diagnostics.Metrics;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
// Remplace BaseAddress par l'URL de ton back-end API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7064/api/")
}); 
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ApiClient>();

builder.Services.AddScoped<DemandeCongeService>();
builder.Services.AddScoped<UserService>();
// Services métier - Enregistrement de tous les services
builder.Services.AddScoped<PlanningCongeService>();
builder.Services.AddScoped<RappelService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddScoped<ValidationService>();

// Service utilitaire pour les notifications
builder.Services.AddScoped<INotificationService, NotificationService>();




// Restaure la session depuis localStorage avant d'afficher l'app
//var auth = builder.Build().Services.GetRequiredService<AuthService>();
//await auth.TryRestoreAsync();

//var authServices = new AuthServices(builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>());


await builder.Build().RunAsync();



// Interface et implémentation pour les notifications
public interface INotificationService
{
    Task ShowSuccessAsync(string message);
    Task ShowErrorAsync(string message);
    Task ShowWarningAsync(string message);
    Task ShowInfoAsync(string message);
}

public class NotificationService : INotificationService
{
    private readonly ISnackbar _snackbar;

    public NotificationService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    public Task ShowSuccessAsync(string message)
    {
        _snackbar.Add(message, Severity.Success);
        return Task.CompletedTask;
    }

    public Task ShowErrorAsync(string message)
    {
        _snackbar.Add(message, Severity.Error);
        return Task.CompletedTask;
    }

    public Task ShowWarningAsync(string message)
    {
        _snackbar.Add(message, Severity.Warning);
        return Task.CompletedTask;
    }

    public Task ShowInfoAsync(string message)
    {
        _snackbar.Add(message, Severity.Info);
        return Task.CompletedTask;
    }
}