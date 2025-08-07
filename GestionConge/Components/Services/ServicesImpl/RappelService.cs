namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;


public class RappelService : IRappelService
{
    private readonly IRappelRepository _repository;

    public RappelService(IRappelRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<RappelDto>> GetAllAsync() => _repository.GetAllAsync();
    public Task<RappelDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<int> CreateAsync(Rappel rappel) => _repository.CreateAsync(rappel);
    public Task<bool> UpdateAsync(Rappel rappel) => _repository.UpdateAsync(rappel);
    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
