namespace GestionConge.Components.Repositories.RepositoriesImpl;

using Dapper;
using GestionConge.Components.Auth;
using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using System.Data;
using System.Security.Cryptography;
using System.Text;

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


    public async Task UpdateRefreshTokenAsync(int userId, string? token, DateTime? expiresUtc)
    {
        // Si token null -> on supprime les colonnes (NULL)
        if (string.IsNullOrEmpty(token))
        {
            var sqlNull = @"
                    UPDATE utilisateurs
                    SET refreshtoken = NULL,
                        refreshtokenexpirydate = NULL
                    WHERE id = @UserId;";
            await _db.ExecuteAsync(sqlNull, new { UserId = userId });
            return;
        }

        // Hacher le refresh token avant stockage (SHA256 hex)
        var hashed = HashToken(token);

        var sql = @"
                UPDATE utilisateurs
                SET refreshtoken = @HashedToken,
                    refreshtokenexpirydate = @ExpiresUtc
                WHERE id = @UserId;";
        await _db.ExecuteAsync(sql, new { HashedToken = hashed, ExpiresUtc = expiresUtc, UserId = userId });
    }

    // -------------------------
    // Méthode demandée: GetByRefreshTokenAsync
    // -------------------------
    /// <summary>
    /// Retourne l'utilisateur qui a le refresh token (après hachage) qui n'est pas expiré.
    /// </summary>
    public async Task<UtilisateurAuth?> GetByRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return null;

        var hashed = HashToken(refreshToken);

        var sql = @"
                SELECT id, nom, email, motdepasse, role, superieurid
                FROM utilisateurs
                WHERE refreshtoken = @HashedToken
                  AND refreshtokenexpirydate IS NOT NULL
                  AND refreshtokenexpirydate > NOW()
                LIMIT 1;
            ";

        return await _db.QueryFirstOrDefaultAsync<UtilisateurAuth>(sql, new { HashedToken = hashed });
    }

    // -------------------------
    // Helper: hash token (SHA256 -> hex)
    // -------------------------
    private static string HashToken(string token)
    {
        // Convert to bytes and compute SHA256 then hex-string (lowercase)
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
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
