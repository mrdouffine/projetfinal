namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;

public class UtilisateurService : IUtilisateurService
{
    private readonly IUtilisateurRepository _repository;

    public UtilisateurService(IUtilisateurRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Utilisateur>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Utilisateur?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<int> CreateAsync(Utilisateur utilisateur) => _repository.CreateAsync(utilisateur);
    public Task<bool> UpdateAsync(Utilisateur utilisateur) => _repository.UpdateAsync(utilisateur);
    public Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId) =>
    _repository.GetSubordonnesAsync(superieurId);

    public async Task<bool> ModifierRoleAsync(int id, string role)
    {
        var utilisateur = await _repository.GetByIdAsync(id);
        if (utilisateur == null) return false;

        utilisateur.Role = role;
        return await _repository.UpdateAsync(utilisateur);
    }

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
