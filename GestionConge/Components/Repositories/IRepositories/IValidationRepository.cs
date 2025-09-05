namespace GestionConge.Components.Repositories.IRepositories;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;

public interface IValidationRepository
{
    Task<IEnumerable<ValidationDto>> GetAllAsync();
    Task<ValidationDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(Validation validation);
    Task<bool> UpdateAsync(Validation validation);
    Task<ValidationDto?> GetByValideurAndDemandeAsync(int valideurId, int demandeId);

    //GetValidationsByValidateurAsync
    Task<IEnumerable<ValidationDto>> GetValidationsByValidateurAsync(int valideurId);

    //GetValidationsAujourdhuiAsync()

    Task<IEnumerable<ValidationDto>> GetValidationsAujourdhuiAsync(int valideurId);

    //récupérer les id des dot
    Task<IEnumerable<int>> GetDotIdsAsync();
    Task<bool> DeleteAsync(int id);
}
