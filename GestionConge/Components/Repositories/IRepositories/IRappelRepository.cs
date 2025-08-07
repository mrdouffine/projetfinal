namespace GestionConge.Components.Repositories.IRepositories;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
public interface IRappelRepository
{
    Task<IEnumerable<RappelDto>> GetAllAsync();
    Task<RappelDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(Rappel rappel);
    Task<bool> UpdateAsync(Rappel rappel);
    Task<bool> DeleteAsync(int id);
}
