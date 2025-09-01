namespace GestionConge.Components.Repositories.IRepositories;

using GestionConge.Components.Auth;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;

public interface IUtilisateurRepository
{
    Task<IEnumerable<Utilisateur>> GetAllAsync();

    //Récupérer les tous les utilisateurs en retournant leurs noms sauf l'Admin
    Task<IEnumerable<Utilisateur>> GetAllUsersNotAdminAsync();
    Task<Utilisateur?> GetByIdAsync(int? id);
    Task<int> CreateAsync(UtilisateurAuth utilisateurAuth);

    Task<UtilisateurAuth?> GetByUserNameAsync(string nom);
    Task<UtilisateurAuth?> GetByEmailAndPasswordAsync(string email, string password);
    Task<bool> UpdateAsync(UtilisateurDto utilisateurDto);
    Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId);

    // Set le supérieur d'un utilisateur
    Task<bool> SetSuperieurAsync(int utilisateurId, int? superieurId);

    Task<int?> GetUtilisateurIdByNameAsync(string nom);
    Task<IEnumerable<Utilisateur>> GetUtilisateursByNameAsync(string nom);
    Task<Utilisateur?> GetByRoleAsync(string role);
    Task<UtilisateurAuth?> GetByEmailAsync(string email);
    Task<bool> DeleteAsync(int id);


    // Méthodes pour la gestion des tokens
    Task UpdateRefreshTokenAsync(int userId, string? token, DateTime? expiresUtc);
    Task<UtilisateurAuth?> GetByRefreshTokenAsync(string refreshToken);
}
