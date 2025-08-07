namespace GestionConge.Components.Repositories.IRepositories;

using GestionConge.Components.Models;

public interface IUtilisateurRepository
{
    Task<IEnumerable<Utilisateur>> GetAllAsync();
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(Utilisateur utilisateur);
    Task<bool> UpdateAsync(Utilisateur utilisateur);
    Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId);
    Task<bool> DeleteAsync(int id);
}
