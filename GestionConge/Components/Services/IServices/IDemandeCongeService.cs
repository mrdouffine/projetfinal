namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;

public interface IDemandeCongeService
{
    Task<IEnumerable<DemandeCongeDto>> GetAllAsync();
    Task<DemandeCongeDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(DemandeConge demande);
    Task<bool> UpdateAsync(DemandeConge demande);
    Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId);
    Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int validateurId);

    Task<bool> DeleteAsync(int id);
}

