using System.Net.Http.Json;
using CongesApp.Models;
public class AuthService
{
    private readonly HttpClient _http;
    public AuthService(HttpClient http) { _http = http; }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        // POST to API /auth/login (adapter to your backend)
        var resp = await _http.PostAsJsonAsync("api/auth/login", model);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        var resp = await _http.PostAsJsonAsync("api/auth/register", model);
        return resp.IsSuccessStatusCode;
    }
}
