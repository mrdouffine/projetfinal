using Blazored.LocalStorage;
using GestionConge.Client.Services;
using GestionConge.Client.Handlers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Remove the default authentication service to use your custom one
// builder.Services.AddApiAuthorization();

// MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    //équivalent de BackgroundOpacity dans Mudblazor avec NET 9
    config.SnackbarConfiguration.BackgroundBlurred = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
});

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Register your custom authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();

// Register the custom delegating handler
builder.Services.AddScoped<CustomAuthorizationHandler>();

// Register your HTTP client and configure it to use your custom handler
builder.Services.AddHttpClient("MonApi", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddHttpMessageHandler<CustomAuthorizationHandler>();

// Services API
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MonApi"));
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ApiClient>();

// Services métier
builder.Services.AddScoped<DemandeCongeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlanningCongeService>();
builder.Services.AddScoped<RappelService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<UtilisateurService>();

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
