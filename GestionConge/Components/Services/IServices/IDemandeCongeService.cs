namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;

public interface IDemandeCongeService
{
    Task<IEnumerable<DemandeCongeDto>> GetAllAsync();
    Task<DemandeCongeDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(DemandeCongeRequestDto demande);
    Task<bool> UpdateAsync(DemandeCongeDto demande);
    Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId);
    Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int validateurId);

    Task<bool> DeleteAsync(int id);
}

