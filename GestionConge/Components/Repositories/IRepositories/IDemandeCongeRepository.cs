using GestionConge.Components.Models;
using GestionConge.Components.DTOs;

namespace GestionConge.Components.Repositories.IRepositories;

public interface IDemandeCongeRepository
{
    Task<IEnumerable<DemandeCongeDto>> GetAllAsync();
    Task<DemandeCongeDto> GetByIdAsync(int id);
    Task<int> CreateAsync(DemandeConge demande);
    Task<bool> UpdateAsync(DemandeConge demande);
    Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId);
    Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int validateurId);
    Task<bool> UpdateStatutAsync(int demandeId, string statut);

    Task<bool> DeleteAsync(int id);
}

