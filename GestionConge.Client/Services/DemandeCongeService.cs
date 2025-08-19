using GestionConge.Client.Models;
namespace GestionConge.Client.Services;

public class DemandeCongeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DemandeCongeService> _logger;

    public DemandeCongeService(HttpClient httpClient, ILogger<DemandeCongeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<DemandeCongeDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/DemandeConge");

            if (response.IsSuccessStatusCode)
            {
                var demandes = await response.Content.ReadFromJsonAsync<IEnumerable<DemandeCongeDto>>();
                return ServiceResult<IEnumerable<DemandeCongeDto>>.Success(demandes ?? new List<DemandeCongeDto>());
            }

            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de toutes les demandes");
            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<DemandeCongeDto>> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/DemandeConge/{id}");

            if (response.IsSuccessStatusCode)
            {
                var demande = await response.Content.ReadFromJsonAsync<DemandeCongeDto>();
                return ServiceResult<DemandeCongeDto>.Success(demande!);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<DemandeCongeDto>.Failure("Demande non trouvée");
            }

            return ServiceResult<DemandeCongeDto>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la demande {Id}", id);
            return ServiceResult<DemandeCongeDto>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<int>> CreateAsync(DemandeCongeRequestDto demande)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/DemandeConge", demande);

            if (response.IsSuccessStatusCode)
            {
                var locationHeader = response.Headers.Location?.ToString();
                if (locationHeader != null && int.TryParse(locationHeader.Split('/').Last(), out int id))
                {
                    return ServiceResult<int>.Success(id);
                }
                return ServiceResult<int>.Success(0); // ID non récupérable mais création réussie
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return ServiceResult<int>.Failure($"Erreur lors de la création: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la demande");
            return ServiceResult<int>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(DemandeCongeDto demande)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/DemandeConge/{demande.Id}", demande);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Demande non trouvée");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la mise à jour: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de la demande {Id}", demande.Id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }

    //GetDemandesEnAttenteAsync
    public async Task<ServiceResult<IEnumerable<DemandeCongeDto>>> GetDemandesEnAttenteAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/DemandeConge/en-attente");
            if (response.IsSuccessStatusCode)
            {
                var demandes = await response.Content.ReadFromJsonAsync<IEnumerable<DemandeCongeDto>>();
                return ServiceResult<IEnumerable<DemandeCongeDto>>.Success(demandes ?? new List<DemandeCongeDto>());
            }
            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des demandes en attente");
            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, DemandeCongeRequestDto demande)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/DemandeConge/{id}", demande);
            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Demande non trouvée");
            }
            return ServiceResult<bool>.Failure($"Erreur lors de la mise à jour: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de la demande {Id}", id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<IEnumerable<DemandeCongeDto>>> GetByUtilisateurAsync(int utilisateurId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/DemandeConge/utilisateur/{utilisateurId}");

            if (response.IsSuccessStatusCode)
            {
                var demandes = await response.Content.ReadFromJsonAsync<IEnumerable<DemandeCongeDto>>();
                return ServiceResult<IEnumerable<DemandeCongeDto>>.Success(demandes ?? new List<DemandeCongeDto>());
            }

            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des demandes de l'utilisateur {UserId}", utilisateurId);
            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<IEnumerable<DemandeCongeDto>>> GetAssignesAsync(int validateurId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/DemandeConge/assignes/{validateurId}");

            if (response.IsSuccessStatusCode)
            {
                var demandes = await response.Content.ReadFromJsonAsync<IEnumerable<DemandeCongeDto>>();
                return ServiceResult<IEnumerable<DemandeCongeDto>>.Success(demandes ?? new List<DemandeCongeDto>());
            }

            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des demandes assignées au validateur {ValidatorId}", validateurId);
            return ServiceResult<IEnumerable<DemandeCongeDto>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/DemandeConge/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Demande non trouvée");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la suppression: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de la demande {Id}", id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }
}


