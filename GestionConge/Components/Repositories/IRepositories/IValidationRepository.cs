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
    Task<bool> DeleteAsync(int id);
}
