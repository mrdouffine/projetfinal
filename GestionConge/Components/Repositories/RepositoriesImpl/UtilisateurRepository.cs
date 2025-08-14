namespace GestionConge.Components.Repositories.RepositoriesImpl;

using Dapper;
using GestionConge.Components.Auth;
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

    public async Task<int> CreateAsync(UtilisateurAuth utilisateurAuth)
    {
        var sql = @"
        INSERT INTO utilisateurs (nom, email, motdepasse, role)
        VALUES (@Nom, @Email, @MotDePasse, @Role)
        RETURNING id";
        return await _db.ExecuteScalarAsync<int>(sql, utilisateurAuth);
    }
    public async Task<UtilisateurAuth?> GetByEmailAndPasswordAsync(string email, string password)
    {
        var sql = "SELECT * FROM utilisateurs WHERE email = @Email AND motdepasse = @MotDePasse";
        return await _db.QueryFirstOrDefaultAsync<UtilisateurAuth>(sql, new { Email = email, MotDePasse = password });
    }

    public Task<UtilisateurAuth?> GetByUserNameAsync(string nom) =>
        _db.QueryFirstOrDefaultAsync<UtilisateurAuth>(
            "SELECT id, nom, email, motdepasse, role FROM utilisateurs WHERE nom = @nom",
            new { nom });
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

    public async Task<UtilisateurAuth?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM utilisateurs WHERE email = @Email";
        return await _db.QueryFirstOrDefaultAsync<UtilisateurAuth>(sql, new { Email = email });
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
