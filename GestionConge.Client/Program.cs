using Blazored.LocalStorage;
using GestionConge.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
// MudBlazor
builder.Services.AddMudServices();

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<UtilisateurService>();


// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();

// HttpClient configuré pour votre API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7064/")
});

// Services API
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ApiClient>();

// Services métier
builder.Services.AddScoped<DemandeCongeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlanningCongeService>();
builder.Services.AddScoped<RappelService>();
builder.Services.AddScoped<ValidationService>();



// Service utilitaire pour les notifications
builder.Services.AddScoped<INotificationService, NotificationService>();

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