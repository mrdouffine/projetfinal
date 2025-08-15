namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.Auth;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
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
    public async Task<int> CreateAsync(UtilisateurAuth utilisateurAuth)
    {
        // ✨ AJOUT : Validation métier
        await ValidateUniqueEmailAsync(utilisateurAuth.Email);
        //await ValidateSuperieurExistsAsync(utilisateurAuth.SuperieurId);

       return await  _repository.CreateAsync(utilisateurAuth);
    }

    public Task<UtilisateurAuth?> GetByEmailAndPasswordAsync(string email, string password) =>
        _repository.GetByEmailAndPasswordAsync(email, password);

    public Task<bool> UpdateAsync(UtilisateurDto utilisateurDto) => _repository.UpdateAsync(utilisateurDto);
    public Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId) =>
    _repository.GetSubordonnesAsync(superieurId);


    private async Task ValidateUniqueEmailAsync(string email)
    {
        // Vérifier que l'email n'existe pas déjà
        var existingUser = await _repository.GetByEmailAsync(email);
        if (existingUser != null)
            throw new InvalidOperationException("Un utilisateur avec cet email existe déjà");
    }

    private async Task ValidateSuperieurExistsAsync(int? superieurId)
    {
        if (superieurId.HasValue)
        {
            var superieur = await _repository.GetByIdAsync(superieurId.Value);
            if (superieur == null)
                throw new InvalidOperationException("Le supérieur spécifié n'existe pas");
        }
    }

    //public async Task<bool> ModifierRoleAsync(int id, string role)
    //{
    //    var utilisateur = await _repository.GetByIdAsync(id);
    //    if (utilisateur == null) return false;

    //    utilisateur.Role = role;
    //    return await _repository.UpdateAsync(utilisateur);
    //}

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
