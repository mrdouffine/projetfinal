namespace GestionConge.Components.Repositories.IRepositories;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;

public interface IUtilisateurRepository
{
    Task<IEnumerable<Utilisateur>> GetAllAsync();
    Task<Utilisateur?> GetByIdAsync(int id);
    Task<int> CreateAsync(UtilisateurRequestDto utilisateurRequestDto);
    Task<bool> UpdateAsync(UtilisateurDto utilisateurDto);
    Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId);
    Task<bool> DeleteAsync(int id);
}
