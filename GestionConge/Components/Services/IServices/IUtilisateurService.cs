namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.Auth;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;

public interface IUtilisateurService
{
    Task<IEnumerable<Utilisateur>> GetAllAsync();

    Task<IEnumerable<Utilisateur>> GetAllUsersNotAdminAsync();
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(UtilisateurAuth utilisateurAuth);
    Task<UtilisateurAuth?> GetByEmailAndPasswordAsync(string email, string password);
    Task<bool> UpdateAsync(UtilisateurDto utilisateurDto);

    Task<bool> SetSuperieurAsync(int utilisateurId, int? superieurId);
    Task<int?> GetUtilisateurIdByNameAsync(string nom);
    Task<IEnumerable<Utilisateur>> GetUtilisateursByNameAsync(string nom);
    Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId);
    //Task<bool> ModifierRoleAsync(int id, string role);

    Task<bool> DeleteAsync(int id);
}
