namespace GestionConge.Client.Services;
using GestionConge.Client.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

public class AuthServices
{
    private const string LocalStorageKey = "currentUser";
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly IJSRuntime _js;

    // Clé pour stocker les données d'authentification dans le localStorage
    private const string AuthStorageKey = "authData";


    private UserSession? _currentUser;

    public event Action? OnChange;

    public AuthServices(IJSRuntime jsRuntime)
    {
        _js = jsRuntime;
    }

    public UserSession? GetCurrentUser() => _currentUser;

    public async Task<bool> RegisterAsync(Models.AuthDtos.RegisterDto dto)
    {
        using var _http = new HttpClient { BaseAddress = new Uri("https://localhost:7064") };
        var res = await _http.PostAsJsonAsync("api/auth/register", dto);
        if (!res.IsSuccessStatusCode) return false;

        // On reçoit les tokens comme pour login
        var auth = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (auth is null) return false;

        await SaveAuthAsync(auth);
        return true;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        using var _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7064") };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        if (!response.IsSuccessStatusCode)
            return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (auth == null) return false;

        // Construire une session utilisateur
        _currentUser = new UserSession
        {
            Id = auth.UserId,
            Email = auth.Email,
            Nom = auth.UserName,
            Role = auth.Role
        };

        // Sauvegarder tokens + infos dans le localStorage
        var json = JsonSerializer.Serialize(auth);
        await _js.InvokeVoidAsync("localStorage.setItem", "authData", json);

        NotifyStateChanged();
        return true;
    }


    //public async Task<bool> RegisterAsync(string nom,string email, string password,string role)
    //{
    //    // Appeler ton backend API pour authentifier

    //    // Exemple simplifié : POST /api/auth/login { email, password }
    //    // Ici on simule la requête, à remplacer par ton vrai appel HTTP

    //    // Exemple avec HttpClient (injecté dans ce service ou passé en paramètre)

    //    var registerSuccess = false;

    //    // Simuler un appel API avec HttpClient (à adapter)
    //    using var _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7064") };

    //    var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new {Nom = nom, Email = email, Password = password, Role = role});
    //    if(response.IsSuccessStatusCode)
    //    {
    //        registerSuccess = true;
    //        //var user = await response.Content.ReadFromJsonAsync<UserSession>();
    //        //_currentUser = user;
    //        //await SaveUserToLocalStorage();
    //        //registerSuccess = true;
    //    }


    //    //// Pour démo, on simule un user  
    //    //if (email == "test@example.com" && password == "password")
    //    //{
    //    //    _currentUser = new UserSession
    //    //    {
    //    //        Id = 1,
    //    //        Email = email,
    //    //        Nom = "Test User",
    //    //        Role = "User"
    //    //    };
    //    //    await SaveUserToLocalStorage();
    //    //    registerSuccess = true;
    //    //}

    //    NotifyStateChanged();

    //    return registerSuccess;
    //}

    public async Task LogoutAsync()
    {
        _currentUser = null;
        await _js.InvokeVoidAsync("localStorage.removeItem", LocalStorageKey);
        NotifyStateChanged();
    }

    private async Task SaveUserToLocalStorage()
    {
        if (_currentUser != null)
        {
            var json = JsonSerializer.Serialize(_currentUser);
            await _js.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
        }
    }

    public async Task LoadUserFromLocalStorageAsync()
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
        if (!string.IsNullOrEmpty(json))
        {
            _currentUser = JsonSerializer.Deserialize<UserSession>(json);
            NotifyStateChanged();
        }
    }
    // ---------- Récupère un access token valide (refresh auto si proche d’expiration) ----------
    public async Task<string> GetAccessTokenAsync()
    {
        var auth = await GetAuthAsync();
        if (auth is null) throw new Exception("Utilisateur non connecté");

        // marge (skew) pour rafraîchir un peu avant l’expiration
        var skew = TimeSpan.FromSeconds(60);
        if (DateTime.UtcNow + skew >= auth.AccessTokenExpires)
        {
            var refreshed = await RefreshTokenAsync(auth);
            if (!refreshed)
            {
                // refresh impossible -> logout
                await LogoutAsync();
                throw new Exception("Session expirée, veuillez vous reconnecter.");
            }
            auth = await GetAuthAsync();
        }

        return auth!.AccessToken;
    }

    // ---------- Appelle /api/auth/refresh en étant thread-safe ----------
    private async Task<bool> RefreshTokenAsync(AuthResponseDto currentAuth)
    {
        var _http = new HttpClient { BaseAddress = new Uri("https://localhost:7064") };
        // si déjà expiré côté refresh -> inutile d’essayer
        if (DateTime.UtcNow >= currentAuth.RefreshTokenExpires)
            return false;

        await _refreshLock.WaitAsync();
        try
        {
            // double-check après le lock
            var latest = await GetAuthAsync();
            if (latest is null) return false;
            if (DateTime.UtcNow + TimeSpan.FromSeconds(60) < latest.AccessTokenExpires)
                return true; // quelqu’un a déjà rafraîchi

            var body = new RefreshRequestDto
            {
                AccessToken = latest.AccessToken,
                RefreshToken = latest.RefreshToken
            };

            var res = await _http.PostAsJsonAsync("api/auth/refresh", body);
            if (!res.IsSuccessStatusCode) return false;

            var newAuth = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (newAuth is null) return false;

            await SaveAuthAsync(newAuth);
            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // ---------- Helpers stockage ----------
    private async Task<AuthResponseDto?> GetAuthAsync()
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", AuthStorageKey);
        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<AuthResponseDto>(json);
    }

    private async Task SaveAuthAsync(AuthResponseDto auth)
    {
       
        // met à jour la session en mémoire
        _currentUser = new UserSession
        {
            Id = auth.UserId,
            Email = auth.Email,
            Nom = auth.UserName,
            Role = auth.Role
        };

        var json = JsonSerializer.Serialize(auth);
        await _js.InvokeVoidAsync("localStorage.setItem", AuthStorageKey, json);
        OnChange?.Invoke();
    }



    private void NotifyStateChanged() => OnChange?.Invoke();
}
