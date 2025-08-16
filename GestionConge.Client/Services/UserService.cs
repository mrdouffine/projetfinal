using GestionConge.Client.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GestionConge.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }


        public async Task<List<UtilisateurDto>?> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<UtilisateurDto>>("api/utilisateurs");
        }

        public async Task<UtilisateurDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<UtilisateurDto>($"api/utilisateurs/{id}");
        }

        public async Task<bool> UpdateAsync(int id, UtilisateurDto utilisateur)
        {
            var response = await _http.PutAsJsonAsync($"api/utilisateurs/{id}", utilisateur);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/utilisateurs/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
