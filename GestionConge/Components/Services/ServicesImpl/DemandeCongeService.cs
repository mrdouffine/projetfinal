using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;

namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
public class DemandeCongeService : IDemandeCongeService
{
    private readonly IDemandeCongeRepository _repository;

    public DemandeCongeService(IDemandeCongeRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<DemandeCongeDto>> GetAllAsync() => _repository.GetAllAsync();
    public Task<DemandeCongeDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<int> CreateAsync(DemandeCongeRequestDto demande) => _repository.CreateAsync(demande);
    public Task<bool> UpdateAsync(DemandeCongeDto demande) => _repository.UpdateAsync(demande);
    public Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId)
    => _repository.GetByUtilisateurIdAsync(utilisateurId);

    public Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int validateurId)
        => _repository.GetAssignesAsync(validateurId);

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}

