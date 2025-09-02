namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;

public interface IValidationService
{
    Task<IEnumerable<ValidationDto>> GetAllAsync();
    Task<ValidationDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(Validation validation);
    Task<bool> UpdateAsync(Validation validation);
    Task<bool> TraiterValidationAsync(ValidationRequestDto request);
    //GetValidationsAujourdhuiAsync()

    Task<IEnumerable<ValidationDto>> GetValidationsAujourdhuiAsync(int valideurId);

    Task<IEnumerable<ValidationDto>> GetValidationsByValidateurAsync(int valideurId);

    Task<bool> DeleteAsync(int id);
}
