using static GestionConge.Client.Models.AuthDtos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GestionConge.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> LoginAsync(LoginDto loginDto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", loginDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", registerDto);
        return response.IsSuccessStatusCode;
    }

    // Ajoute d'autres méthodes si besoin (Logout, RefreshToken, etc.)
}
