using System.Net.Http.Json;
using CongesApp.Models;

namespace GestionConge.Client.Services
{
    public class CongeService
    {
        private readonly HttpClient _http;

        public CongeService(HttpClient http)
        {
            _http = http;
        }

        // Récupère les demandes en attente
        public async Task<List<DemandeCongeModel>> GetDemandesEnAttenteAsync()
        {
            return await _http.GetFromJsonAsync<List<
                    DemandeCongeModel>>("api/conges/attente");
        }

        // Valider une demande
        public async Task<bool> ApproveAsync(int id)
        {
            var response = await _http.PostAsync($"api/conges/{id}/approve", null);
            return response.IsSuccessStatusCode;
        }

        // Refuser une demande
        public async Task<bool> RejectAsync(int id)
        {
            var response = await _http.PostAsync($"api/conges/{id}/reject", null);
            return response.IsSuccessStatusCode;
        }
    }
}
