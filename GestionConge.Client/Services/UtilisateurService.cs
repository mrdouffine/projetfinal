using GestionConge.Client.Models;

namespace GestionConge.Client.Services;

public class UtilisateurService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UtilisateurService> _logger;

    public UtilisateurService(HttpClient httpClient, ILogger<UtilisateurService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<UtilisateurDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Utilisateur");

            if (response.IsSuccessStatusCode)
            {
                var utilisateurs = await response.Content.ReadFromJsonAsync<IEnumerable<UtilisateurDto>>();
                return ServiceResult<IEnumerable<UtilisateurDto>>.Success(utilisateurs ?? new List<UtilisateurDto>());
            }

            return ServiceResult<IEnumerable<UtilisateurDto>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de tous les utilisateurs");
            return ServiceResult<IEnumerable<UtilisateurDto>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<UtilisateurDto>> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Utilisateur/{id}");

            if (response.IsSuccessStatusCode)
            {
                var utilisateur = await response.Content.ReadFromJsonAsync<UtilisateurDto>();
                return ServiceResult<UtilisateurDto>.Success(utilisateur!);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<UtilisateurDto>.Failure("Utilisateur non trouvé");
            }

            return ServiceResult<UtilisateurDto>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {Id}", id);
            return ServiceResult<UtilisateurDto>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<int>> CreateAsync(UtilisateurAuth utilisateurAuth)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Utilisateur", utilisateurAuth);

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
            _logger.LogError(ex, "Erreur lors de la création de l'utilisateur");
            return ServiceResult<int>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(UtilisateurDto utilisateur)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Utilisateur/{utilisateur.Id}", utilisateur);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Utilisateur non trouvé");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la mise à jour: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur {Id}", utilisateur.Id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Utilisateur/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Utilisateur non trouvé");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la suppression: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur {Id}", id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
            
        }
    }
}    // End of code snippet