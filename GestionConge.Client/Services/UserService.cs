using System.Net.Http.Json;
using CongesApp.Models;
public class UserService
{
    private readonly HttpClient _http;
    public UserService(HttpClient http) { _http = http; }

    public async Task<List<UserModel>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<UserModel>>("api/users") ?? new();
    }
}
