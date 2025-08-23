using Blazored.LocalStorage;
using GestionConge.Client.Models;
using GestionConge.Client.Services;

namespace GestionConge.Client.Services;

public class AuthService
{
    private const string AuthStorageKey = "authData";  // objet complet (pour refresh, etc.)
    private const string TokenKey = "authToken";       // pur JWT (lu par CustomAuthStateProvider)

    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly ILocalStorageService _localStorage;
    private readonly CustomAuthStateProvider _authProvider;
    private readonly HttpClient _http; // injecté (conseillé)

    private UserSession? _currentUser;
    public event Action? OnChange;

    public AuthService(ILocalStorageService localStorage,
                       CustomAuthStateProvider authProvider,
                       HttpClient httpFactory)
    {
        _localStorage = localStorage;
        _authProvider = authProvider;
        _http = httpFactory;
        _http.BaseAddress = new Uri("https://localhost:7064/");
    }

    public UserSession? GetCurrentUser() => _currentUser;

    public async Task TryRestoreAsync()
    {
        if (_currentUser is not null) return;

        var auth = await _localStorage.GetItemAsync<AuthResponseDto>(AuthStorageKey);
        if (auth is null) return;

        _currentUser = new UserSession
        {
            Id = auth.UserId,
            Email = auth.Email,
            Nom = auth.UserName,
            Role = auth.Role
        };

        // S’assure que le provider a bien un token (au cas où)
        var existingToken = await _localStorage.GetItemAsStringAsync(TokenKey);
        if (string.IsNullOrWhiteSpace(existingToken) && !string.IsNullOrWhiteSpace(auth.AccessToken))
        {
            await _localStorage.SetItemAsync(TokenKey, auth.AccessToken);
            await _authProvider.NotifyUserAuthenticationAsync(auth.AccessToken);
        }

        OnChange?.Invoke();
    }

    public async Task<bool> RegisterAsync(AuthDtos.RegisterDto dto)
    {
        var res = await _http.PostAsJsonAsync("api/Auth/register", dto);
        if (!res.IsSuccessStatusCode) return false;

        var auth = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (auth is null) return false;

        await SaveAuthAsync(auth, notifyProvider: true);
        return true;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", new { Email = email, MotDePasse = password });
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (auth is null) return false;

        await SaveAuthAsync(auth, notifyProvider: true);
        OnChange?.Invoke();
        return true;
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        await _localStorage.RemoveItemAsync(AuthStorageKey);
        await _localStorage.RemoveItemAsync(TokenKey);

        await _authProvider.NotifyUserLogoutAsync();
        OnChange?.Invoke();
    }

    // --- Tokens ---

    public async Task<string> GetAccessTokenAsync()
    {
        var auth = await _localStorage.GetItemAsync<AuthResponseDto>(AuthStorageKey);
        if (auth is null || string.IsNullOrEmpty(auth.AccessToken))
            throw new Exception("Utilisateur non connecté");

        var skew = TimeSpan.FromSeconds(60);
        if (DateTime.UtcNow + skew >= auth.AccessTokenExpires)
        {
            var refreshed = await RefreshTokenAsync(auth);
            if (!refreshed)
            {
                await LogoutAsync();
                throw new Exception("Session expirée, veuillez vous reconnecter.");
            }
            auth = await _localStorage.GetItemAsync<AuthResponseDto>(AuthStorageKey);
        }

        return auth!.AccessToken!;
    }

    private async Task<bool> RefreshTokenAsync(AuthResponseDto currentAuth)
    {
        if (DateTime.UtcNow >= currentAuth.RefreshTokenExpires) return false;

        await _refreshLock.WaitAsync();
        try
        {
            // double-check après lock
            var latest = await _localStorage.GetItemAsync<AuthResponseDto>(AuthStorageKey);
            if (latest is null) return false;
            if (DateTime.UtcNow + TimeSpan.FromSeconds(60) < latest.AccessTokenExpires)
                return true;

            var res = await _http.PostAsJsonAsync("api/Auth/refresh", new
            {
                accessToken = latest.AccessToken,
                refreshToken = latest.RefreshToken
            });
            if (!res.IsSuccessStatusCode) return false;

            var newAuth = await res.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (newAuth is null) return false;

            await SaveAuthAsync(newAuth, notifyProvider: true);
            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // --- Helpers ---

    private async Task SaveAuthAsync(AuthResponseDto auth, bool notifyProvider)
    {
        // 1) on garde l’objet complet pour le refresh
        await _localStorage.SetItemAsync(AuthStorageKey, auth);

        // 2) on met le pur JWT à disposition du provider
        if (!string.IsNullOrWhiteSpace(auth.AccessToken))
            await _localStorage.SetItemAsync(TokenKey, auth.AccessToken);

        // 3) maj session en mémoire
        _currentUser = new UserSession
        {
            Id = auth.UserId,
            Email = auth.Email,
            Nom = auth.UserName,
            Role = auth.Role
        };

        if (notifyProvider && !string.IsNullOrWhiteSpace(auth.AccessToken))
            await _authProvider.NotifyUserAuthenticationAsync(auth.AccessToken);

        OnChange?.Invoke();
    }
}
