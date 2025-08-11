using GestionConge.Client.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GestionConge.Client.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        return await _http.GetFromJsonAsync<T>(url);
    }

    public async Task<List<RappelDto>?> GetNotificationsAsync()
    {
        return await _http.GetFromJsonAsync<List<RappelDto>>("api/rappels");
    }

    public async Task<List<ValidationDto>?> GetValidationsAsync()
    {
        return await _http.GetFromJsonAsync<List<ValidationDto>>("api/validations");
    }


}
