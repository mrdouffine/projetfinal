namespace GestionConge.Components.Repositories.RepositoriesImpl;

using Dapper;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using System.Data;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly IDbConnection _db;

    public UtilisateurRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Utilisateur>> GetAllAsync()
    {
        var sql = "SELECT * FROM utilisateurs";
        return await _db.QueryAsync<Utilisateur>(sql);
    }

    public async Task<Utilisateur?> GetByIdAsync(int? id)
    {
        var sql = "SELECT * FROM utilisateurs WHERE id = @Id";
        return await _db.QueryFirstOrDefaultAsync<Utilisateur>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(UtilisateurRequestDto utilisateurRequestDto)
    {
        var sql = @"
        INSERT INTO utilisateurs (nom, email, motdepasse, role, superieurid)
        VALUES (@Nom, @Email, @MotDePasse, @Role, @SuperieurId)
        RETURNING id";
        return await _db.ExecuteScalarAsync<int>(sql, utilisateurRequestDto);
    }

    public async Task<bool> UpdateAsync(UtilisateurDto utilisateurDto)
    {
        //var sql = @"
        //UPDATE utilisateurs
        //SET nom = @Nom, email = @Email, motdepasse = @MotDePasse, role = @Role, superieurid = @SuperieurId
        //WHERE id = @Id";

        // Si tu veux inclure le mot de passe conditionnellement :
        var sql = string.IsNullOrEmpty(utilisateurDto.MotDePasse)
            ? "UPDATE utilisateurs SET nom = @Nom, email = @Email, role = @Role, superieurid = @SuperieurId WHERE id = @Id"
            : "UPDATE utilisateurs SET nom = @Nom, email = @Email, motdepasse = @MotDePasse, role = @Role, superieurid = @SuperieurId WHERE id = @Id";
        var rows = await _db.ExecuteAsync(sql, utilisateurDto);
        return rows > 0;
    }

    // Nouvelle méthode pour récupérer les subordonnés
    public async Task<IEnumerable<Utilisateur>> GetSubordonnesAsync(int superieurId)
    {
        var sql = "SELECT * FROM utilisateurs WHERE superieurid = @SuperieurId";
        return await _db.QueryAsync<Utilisateur>(sql, new { SuperieurId = superieurId });
    }

    // ✨ AJOUT dans IUtilisateurRepository et UtilisateurRepository
    public async Task<Utilisateur?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM utilisateurs WHERE email = @Email";
        return await _db.QueryFirstOrDefaultAsync<Utilisateur>(sql, new { Email = email });
    }

    public async Task<Utilisateur?> GetByRoleAsync(string role)
    {
        var sql = "SELECT * FROM utilisateurs WHERE role = @Role AND superieurid IS NULL LIMIT 1";
        return await _db.QueryFirstOrDefaultAsync<Utilisateur>(sql, new { Role = role });
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "DELETE FROM utilisateurs WHERE id = @Id";
        var rows = await _db.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
