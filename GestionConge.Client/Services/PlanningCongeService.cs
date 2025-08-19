using GestionConge.Client.Models;
namespace GestionConge.Client.Services;

public class PlanningCongeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlanningCongeService> _logger;

    public PlanningCongeService(HttpClient httpClient, ILogger<PlanningCongeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<PlanningConge>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/PlanningConge");

            if (response.IsSuccessStatusCode)
            {
                var plannings = await response.Content.ReadFromJsonAsync<IEnumerable<PlanningConge>>();
                return ServiceResult<IEnumerable<PlanningConge>>.Success(plannings ?? new List<PlanningConge>());
            }

            return ServiceResult<IEnumerable<PlanningConge>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de tous les plannings");
            return ServiceResult<IEnumerable<PlanningConge>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<PlanningConge>> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/PlanningConge/{id}");

            if (response.IsSuccessStatusCode)
            {
                var planning = await response.Content.ReadFromJsonAsync<PlanningConge>();
                return ServiceResult<PlanningConge>.Success(planning!);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<PlanningConge>.Failure("Planning non trouvé");
            }

            return ServiceResult<PlanningConge>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du planning {Id}", id);
            return ServiceResult<PlanningConge>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<int>> CreateAsync(PlanningConge planning)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/PlanningConge", planning);

            if (response.IsSuccessStatusCode)
            {
                var locationHeader = response.Headers.Location?.ToString();
                if (locationHeader != null && int.TryParse(locationHeader.Split('/').Last(), out int id))
                {
                    return ServiceResult<int>.Success(id);
                }
                return ServiceResult<int>.Success(0);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return ServiceResult<int>.Failure($"Erreur lors de la création: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du planning");
            return ServiceResult<int>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(PlanningConge planning)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/PlanningConge/{planning.Id}", planning);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Planning non trouvé");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la mise à jour: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du planning {Id}", planning.Id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<string>> PlanifierAsync(PlanningConge planning)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/PlanningConge/planifier", planning);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<string>.Success("Planning créé avec succès");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<string>.Failure("Impossible de planifier ces congés");
            }

            return ServiceResult<string>.Failure($"Erreur lors de la planification: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la planification");
            return ServiceResult<string>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<IEnumerable<PlanningConge>>> GetByUtilisateurAsync(int utilisateurId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/PlanningConge/utilisateur/{utilisateurId}");

            if (response.IsSuccessStatusCode)
            {
                var plannings = await response.Content.ReadFromJsonAsync<IEnumerable<PlanningConge>>();
                return ServiceResult<IEnumerable<PlanningConge>>.Success(plannings ?? new List<PlanningConge>());
            }

            return ServiceResult<IEnumerable<PlanningConge>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des plannings de l'utilisateur {UserId}", utilisateurId);
            return ServiceResult<IEnumerable<PlanningConge>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<SoldeCongeInfo>> GetSoldeRestantAsync(int utilisateurId, int annee)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/PlanningConge/solde/{utilisateurId}/{annee}");

            if (response.IsSuccessStatusCode)
            {
                var solde = await response.Content.ReadFromJsonAsync<SoldeCongeInfo>();
                return ServiceResult<SoldeCongeInfo>.Success(solde!);
            }

            return ServiceResult<SoldeCongeInfo>.Failure($"Erreur lors de la récupération du solde: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du solde pour l'utilisateur {UserId} année {Year}", utilisateurId, annee);
            return ServiceResult<SoldeCongeInfo>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/PlanningConge/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Planning non trouvé");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la suppression: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du planning {Id}", id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }
}

public class SoldeCongeInfo
{
    public int AnneeReference { get; set; }
    public int TotalPlanifie { get; set; }
    public int SoldeRestant { get; set; }
}


