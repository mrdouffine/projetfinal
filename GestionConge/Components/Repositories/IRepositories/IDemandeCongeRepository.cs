using GestionConge.Components.Models;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;

namespace GestionConge.Components.Repositories.IRepositories;

public interface IDemandeCongeRepository
{
    Task<IEnumerable<DemandeCongeDto>> GetAllAsync();
    Task<DemandeCongeDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(DemandeCongeRequestDto demande);
    Task<bool> UpdateAsync(DemandeCongeDto demande);
    Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId);
    Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int validateurId);
    Task<bool> UpdateStatutAsync(int demandeId, string statut);

    Task<bool> DeleteAsync(int id);
}

