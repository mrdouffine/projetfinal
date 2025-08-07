namespace GestionConge.Components.Services.IServices
{
    using GestionConge.Components.DTOs;
    using GestionConge.Components.Models;

    public interface IRappelService
    {
        Task<IEnumerable<RappelDto>> GetAllAsync();
        Task<RappelDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(Rappel rappel);
        Task<bool> UpdateAsync(Rappel rappel);
        Task<bool> DeleteAsync(int id);
    }

}
