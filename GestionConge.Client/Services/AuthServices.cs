namespace GestionConge.Client.Services;
using System.Net.Http.Json;
using GestionConge.Client.Models;
using Microsoft.JSInterop;
using System.Text.Json;

public class AuthServices
{
    private const string LocalStorageKey = "currentUser";
    private readonly IJSRuntime _jsRuntime;

    private UserSession? _currentUser;

    public event Action? OnChange;

    public AuthServices(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public UserSession? GetCurrentUser() => _currentUser;

    public async Task<bool> LoginAsync(string email, string password)
    {
        // Appeler ton backend API pour authentifier

        // Exemple simplifié : POST /api/auth/login { email, password }
        // Ici on simule la requête, à remplacer par ton vrai appel HTTP

        // Exemple avec HttpClient (injecté dans ce service ou passé en paramètre)

        var loginSuccess = false;

        // Simuler un appel API avec HttpClient (à adapter)
        using var _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7064") };

        var response = await _httpClient.PostAsJsonAsync("/api/utilisateur/login", new { Email = email, Password = password });
        if(response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserSession>();
            _currentUser = user;
            await SaveUserToLocalStorage();
            loginSuccess = true;
        }
        

        // Pour démo, on simule un user  
        if (email == "test@example.com" && password == "password")
        {
            _currentUser = new UserSession
            {
                Id = 1,
                Email = email,
                Nom = "Test User",
                Role = "User"
            };
            await SaveUserToLocalStorage();
            loginSuccess = true;
        }

        NotifyStateChanged();

        return loginSuccess;
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKey);
        NotifyStateChanged();
    }

    private async Task SaveUserToLocalStorage()
    {
        if (_currentUser != null)
        {
            var json = JsonSerializer.Serialize(_currentUser);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
        }
    }

    public async Task LoadUserFromLocalStorageAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
        if (!string.IsNullOrEmpty(json))
        {
            _currentUser = JsonSerializer.Deserialize<UserSession>(json);
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
