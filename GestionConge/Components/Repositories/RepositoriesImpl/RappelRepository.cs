namespace GestionConge.Components.Repositories.RepositoriesImpl;

using Dapper;
using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using System.Data;

public class RappelRepository : IRappelRepository
{
    private readonly IDbConnection _db;

    public RappelRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RappelDto>> GetAllAsync()
    {
        var sql = @"
        SELECT r.id, r.message, r.date_rappel,
               u.id AS UtilisateurId, u.nom AS NomUtilisateur, u.email AS EmailUtilisateur
        FROM rappels r
        JOIN utilisateurs u ON r.utilisateurid = u.id
        ORDER BY r.date_rappel DESC;
        ";

        return await _db.QueryAsync<RappelDto>(sql);
    }

    public async Task<RappelDto?> GetByIdAsync(int id)
    {
        var sql = @"
        SELECT r.id, r.message, r.date_rappel,
               u.id AS UtilisateurId, u.nom AS NomUtilisateur, u.email AS EmailUtilisateur
        FROM rappels r
        JOIN utilisateurs u ON r.utilisateurid = u.id
        WHERE r.id = @Id;
        ";

        return await _db.QueryFirstOrDefaultAsync<RappelDto>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(Rappel rappel)
    {
        var sql = @"
        INSERT INTO rappels (message, date_rappel, utilisateurid)
        VALUES (@Message, @DateRappel, @UtilisateurId)
        RETURNING id;
        ";

        return await _db.ExecuteScalarAsync<int>(sql, rappel);
    }

    public async Task<bool> UpdateAsync(Rappel rappel)
    {
        var sql = @"
        UPDATE rappels
        SET message = @Message,
            date_rappel = @DateRappel
        WHERE id = @Id;
        ";

        return await _db.ExecuteAsync(sql, rappel) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "DELETE FROM rappels WHERE id = @Id;";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
