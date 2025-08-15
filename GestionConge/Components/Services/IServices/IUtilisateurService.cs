namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.Auth;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;

public interface IUtilisateurService
{
    Task<IEnumerable<Utilisateur>> GetAllAsync();
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(UtilisateurAuth utilisateurAuth);
    Task<UtilisateurAuth?> GetByEmailAndPasswordAsync(string email, string password);
    Task<bool> UpdateAsync(UtilisateurDto utilisateurDto);
    Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId);
    //Task<bool> ModifierRoleAsync(int id, string role);

    Task<bool> DeleteAsync(int id);
}
