namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.Models;

public interface IUtilisateurService
{
    Task<IEnumerable<Utilisateur>> GetAllAsync();
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(Utilisateur utilisateur);
    Task<bool> UpdateAsync(Utilisateur utilisateur);
    Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId);
    Task<bool> ModifierRoleAsync(int id, string role);

    Task<bool> DeleteAsync(int id);
}
