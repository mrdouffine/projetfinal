using GestionConge.Client.Models;


namespace GestionConge.Client.Services;

public class RappelService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RappelService> _logger;

    public RappelService(HttpClient httpClient, ILogger<RappelService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<Rappel>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Rappel");

            if (response.IsSuccessStatusCode)
            {
                var rappels = await response.Content.ReadFromJsonAsync<IEnumerable<Rappel>>();
                return ServiceResult<IEnumerable<Rappel>>.Success(rappels ?? new List<Rappel>());
            }

            return ServiceResult<IEnumerable<Rappel>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de tous les rappels");
            return ServiceResult<IEnumerable<Rappel>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<Rappel>> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Rappel/{id}");

            if (response.IsSuccessStatusCode)
            {
                var rappel = await response.Content.ReadFromJsonAsync<Rappel>();
                return ServiceResult<Rappel>.Success(rappel!);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<Rappel>.Failure("Rappel non trouvé");
            }

            return ServiceResult<Rappel>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du rappel {Id}", id);
            return ServiceResult<Rappel>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<int>> CreateAsync(Rappel rappel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Rappel", rappel);

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
            _logger.LogError(ex, "Erreur lors de la création du rappel");
            return ServiceResult<int>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(Rappel rappel)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Rappel/{rappel.Id}", rappel);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Rappel non trouvé");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la mise à jour: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du rappel {Id}", rappel.Id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Rappel/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ServiceResult<bool>.Success(true);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ServiceResult<bool>.Failure("Rappel non trouvé");
            }

            return ServiceResult<bool>.Failure($"Erreur lors de la suppression: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du rappel {Id}", id);
            return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<IEnumerable<Rappel>>> GetRappelsActifsAsync()
    {
        try
        {
            // Récupérer tous les rappels et filtrer côté client pour les rappels actifs
            var allRappelsResult = await GetAllAsync();

            if (allRappelsResult.IsSuccess && allRappelsResult.Data != null)
            {
                var rappelsActifs = allRappelsResult.Data
                    .Where(r => r.DateEcheance >= DateTime.Now)
                    .OrderBy(r => r.DateEcheance)
                    .ToList();

                return ServiceResult<IEnumerable<Rappel>>.Success(rappelsActifs);
            }

            return ServiceResult<IEnumerable<Rappel>>.Failure(allRappelsResult.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des rappels actifs");
            return ServiceResult<IEnumerable<Rappel>>.Failure("Erreur de connexion au serveur");
        }
    }

    public async Task<ServiceResult<IEnumerable<Rappel>>> GetRappelsUtilisateurAsync(int utilisateurId)
    {
        try
        {
            var allRappelsResult = await GetAllAsync();

            if (allRappelsResult.IsSuccess && allRappelsResult.Data != null)
            {
                var rappelsUtilisateur = allRappelsResult.Data
                    .Where(r => r.UtilisateurId == utilisateurId)
                    .OrderBy(r => r.DateEcheance)
                    .ToList();

                return ServiceResult<IEnumerable<Rappel>>.Success(rappelsUtilisateur);
            }

            return ServiceResult<IEnumerable<Rappel>>.Failure(allRappelsResult.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des rappels de l'utilisateur {UserId}", utilisateurId);
            return ServiceResult<IEnumerable<Rappel>>.Failure("Erreur de connexion au serveur");
        }
    }

    //public async Task<ServiceResult<IEnumerable<NotificationDto>>> GetNotificationsUtilisateurAsync(int utilisateurId)
    //{
    //    try
    //    {
    //        var allRappelsResult = await GetAllAsync();

    //        if (allRappelsResult.IsSuccess && allRappelsResult.Data != null)
    //        {
    //            var rappelsUtilisateur = allRappelsResult.Data
    //                .Where(r => r.UtilisateurId == utilisateurId)
    //                .OrderBy(r => r.DateEcheance)
    //                .ToList();

    //            return ServiceResult<IEnumerable<NotificationDto>>.Success(rappelsUtilisateur);
    //        }

    //        return ServiceResult<IEnumerable<NotificationDto>>.Failure(allRappelsResult.ErrorMessage);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Erreur lors de la récupération des rappels de l'utilisateur {UserId}", utilisateurId);
    //        return ServiceResult<IEnumerable<NotificationDto>>.Failure("Erreur de connexion au serveur");
    //    }
    //}

}
