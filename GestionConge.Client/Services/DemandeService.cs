using GestionConge.Client.Models;
using System.Net.Http.Json;

namespace GestionConge.Client.Services
{
    public class DemandeService
    {
        private readonly HttpClient _api;

        public DemandeService(HttpClient api)
        {
            _api = api;
        }

        public async Task<IEnumerable<DemandeCongeDto>> GetDemandesAsync()
        {
            return await _api.GetFromJsonAsync<IEnumerable<DemandeCongeDto>>("api/demandes");
        }
    }
}
