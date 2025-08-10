using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;

namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
public class DemandeCongeService : IDemandeCongeService
{
    private readonly IDemandeCongeRepository _repository;

    public DemandeCongeService(IDemandeCongeRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<DemandeCongeDto>> GetAllAsync() => _repository.GetAllAsync();
    public Task<DemandeCongeDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public async Task<int> CreateAsync(DemandeCongeRequestDto demande)
    {
        ValidateDates(demande.DateDebut, demande.DateFin);
        await ValidateNoOverlapAsync(demande.UtilisateurId, demande.DateDebut, demande.DateFin);

        return await _repository.CreateAsync(demande);
    }

    private void ValidateDates(DateTime debut, DateTime fin)
    {
        if (debut >= fin)
            throw new ArgumentException("La date de fin doit être postérieure à la date de début");

        if (debut < DateTime.Today)
            throw new ArgumentException("Impossible de créer une demande pour une date passée");
    }

    private async Task ValidateNoOverlapAsync(int utilisateurId, DateTime debut, DateTime fin)
    {
        // Vérifier qu'il n'y a pas de chevauchement avec des demandes validées
        var demandesExistantes = await _repository.GetByUtilisateurIdAsync(utilisateurId);

        var overlap = demandesExistantes.Any(d =>
            d.Statut == "Validé" &&
            debut < d.DateFin && fin > d.DateDebut);

        if (overlap)
            throw new InvalidOperationException("Cette période chevauche avec des congés déjà validés");
    }
    public Task<bool> UpdateAsync(DemandeCongeDto demande) => _repository.UpdateAsync(demande);
    public Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId)
    => _repository.GetByUtilisateurIdAsync(utilisateurId);

    public Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int validateurId)
        => _repository.GetAssignesAsync(validateurId);

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}

