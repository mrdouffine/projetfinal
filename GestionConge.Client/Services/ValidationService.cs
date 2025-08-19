using GestionConge.Client.Models;

namespace GestionConge.Client.Services
{
    public class ValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(HttpClient httpClient, ILogger<ValidationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<Validation>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Validation");

                if (response.IsSuccessStatusCode)
                {
                    var validations = await response.Content.ReadFromJsonAsync<IEnumerable<Validation>>();
                    return ServiceResult<IEnumerable<Validation>>.Success(validations ?? new List<Validation>());
                }

                return ServiceResult<IEnumerable<Validation>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les validations");
                return ServiceResult<IEnumerable<Validation>>.Failure("Erreur de connexion au serveur");
            }
        }

        public async Task<ServiceResult<Validation>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Validation/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var validation = await response.Content.ReadFromJsonAsync<Validation>();
                    return ServiceResult<Validation>.Success(validation!);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ServiceResult<Validation>.Failure("Validation non trouvée");
                }

                return ServiceResult<Validation>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la validation {Id}", id);
                return ServiceResult<Validation>.Failure("Erreur de connexion au serveur");
            }
        }

        public async Task<ServiceResult<int>> CreateAsync(Validation validation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Validation", validation);

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
                _logger.LogError(ex, "Erreur lors de la création de la validation");
                return ServiceResult<int>.Failure("Erreur de connexion au serveur");
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(Validation validation)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Validation/{validation.Id}", validation);

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Success(true);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ServiceResult<bool>.Failure("Validation non trouvée");
                }

                return ServiceResult<bool>.Failure($"Erreur lors de la mise à jour: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la validation {Id}", validation.Id);
                return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
            }
        }

        //GetValidationsAujourdhuiAsync
        public async Task<ServiceResult<IEnumerable<Validation>>> GetValidationsAujourdhuiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Validation/aujourdhui");
                if (response.IsSuccessStatusCode)
                {
                    var validations = await response.Content.ReadFromJsonAsync<IEnumerable<Validation>>();
                    return ServiceResult<IEnumerable<Validation>>.Success(validations ?? new List<Validation>());
                }
                return ServiceResult<IEnumerable<Validation>>.Failure($"Erreur lors de la récupération: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des validations d'aujourd'hui");
                return ServiceResult<IEnumerable<Validation>>.Failure("Erreur de connexion au serveur");
            }
        }
        public async Task<ServiceResult<string>> TraiterValidationAsync(ValidationRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Validation/traiter", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    return ServiceResult<string>.Success("Validation traitée avec succès");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return ServiceResult<string>.Failure($"Erreur lors du traitement: {errorContent}");
                }

                return ServiceResult<string>.Failure($"Erreur lors du traitement: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de la validation");
                return ServiceResult<string>.Failure("Erreur de connexion au serveur");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Validation/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Success(true);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ServiceResult<bool>.Failure("Validation non trouvée");
                }

                return ServiceResult<bool>.Failure($"Erreur lors de la suppression: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la validation {Id}", id);
                return ServiceResult<bool>.Failure("Erreur de connexion au serveur");
            }
        }

        public async Task<ServiceResult<string>> ApprouverDemandeAsync(int demandeId, int validateurId, string? commentaire = null)
        {
            var request = new ValidationRequestDto
            {
                DemandeCongeId = demandeId,
                ValideurId = validateurId,
                Statut = "Validé",
                Commentaire = commentaire ?? string.Empty
            };

            return await TraiterValidationAsync(request);
        }

        public async Task<ServiceResult<string>> RejeterDemandeAsync(int demandeId, int validateurId, string commentaire)
        {
            var request = new ValidationRequestDto
            {
                DemandeCongeId = demandeId,
                ValideurId = validateurId,
                Statut = "Rejeté",
                Commentaire = commentaire
            };

            return await TraiterValidationAsync(request);
        }

        public async Task<ServiceResult<IEnumerable<Validation>>> GetValidationsEnAttenteAsync()
        {
            try
            {
                var allValidationsResult = await GetAllAsync();

                if (allValidationsResult.IsSuccess && allValidationsResult.Data != null)
                {
                    var validationsEnAttente = allValidationsResult.Data
                        .Where(v => v.Statut == "En attente")
                        .OrderBy(v => v.DateValidation)
                        .ToList();

                    return ServiceResult<IEnumerable<Validation>>.Success(validationsEnAttente);
                }

                return ServiceResult<IEnumerable<Validation>>.Failure(allValidationsResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des validations en attente");
                return ServiceResult<IEnumerable<Validation>>.Failure("Erreur lors du traitement");
            }
        }

        //GetPendingValidationsCountAsync
        public async Task<ServiceResult<int>> GetPendingValidationsCountAsync()
        {
            try
            {
                var allValidationsResult = await GetAllAsync();
                if (allValidationsResult.IsSuccess && allValidationsResult.Data != null)
                {
                    var count = allValidationsResult.Data.Count(v => v.Statut == "En attente");
                    return ServiceResult<int>.Success(count);
                }
                return ServiceResult<int>.Failure(allValidationsResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de validations en attente");
                return ServiceResult<int>.Failure("Erreur lors du traitement");
            }
        }

        public async Task<ServiceResult<IEnumerable<Validation>>> GetValidationsByValidateurAsync(int validateurId)
        {
            try
            {
                var allValidationsResult = await GetAllAsync();

                if (allValidationsResult.IsSuccess && allValidationsResult.Data != null)
                {
                    var validationsValidateur = allValidationsResult.Data
                        .Where(v => v.ValidateurId == validateurId)
                        .OrderByDescending(v => v.DateValidation)
                        .ToList();

                    return ServiceResult<IEnumerable<Validation>>.Success(validationsValidateur);
                }

                return ServiceResult<IEnumerable<Validation>>.Failure(allValidationsResult.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des validations du validateur {ValidatorId}", validateurId);
                return ServiceResult<IEnumerable<Validation>>.Failure("Erreur lors du traitement");
            }
        }
    }
}