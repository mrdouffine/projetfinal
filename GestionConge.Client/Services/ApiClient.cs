using GestionConge.Client.Models;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GestionConge.Client.Services;

public class ApiClient
{


    private readonly HttpClient _httpClient;
    private readonly AuthServices _auth;

    public ApiClient(HttpClient httpClient, AuthServices auth)
    {
        _httpClient = httpClient;
        _auth = auth;
    }

    public async Task<HttpClient> GetAuthorizedClientAsync()
    {
        var token = await _auth.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return _httpClient;
    }

    public async Task<T?> GetAsync<T>(string url) =>
       await (await GetAuthorizedClientAsync()).GetFromJsonAsync<T>(url);

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body) =>
        await (await GetAuthorizedClientAsync()).PostAsJsonAsync(url, body);

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T body) =>
        await (await GetAuthorizedClientAsync()).PutAsJsonAsync(url, body);

    public async Task<HttpResponseMessage> DeleteAsync(string url) =>
        await (await GetAuthorizedClientAsync()).DeleteAsync(url);

}
