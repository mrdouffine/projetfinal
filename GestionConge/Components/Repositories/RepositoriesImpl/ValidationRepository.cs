namespace GestionConge.Components.Repositories.RepositoriesImpl;

using Dapper;
using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using System.Data;

public class ValidationRepository : IValidationRepository
{
    private readonly IDbConnection _db;

    public ValidationRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ValidationDto>> GetAllAsync()
    {
        var sql = @"
        SELECT v.id, v.demandecongeid, v.statut, v.commentaire, v.datevalidation,
               u.id as ValideurId, u.nom as NomValideur, u.email as EmailValideur
        FROM validations v
        JOIN utilisateurs u ON v.valideurid = u.id
        ORDER BY v.datevalidation DESC;
        ";

        return await _db.QueryAsync<ValidationDto>(sql);
    }

    public async Task<ValidationDto?> GetByIdAsync(int id)
    {
        var sql = @"
        SELECT v.id, v.demandecongeid, v.statut, v.commentaire, v.datevalidation,
               u.id as ValideurId, u.nom as NomValideur, u.email as EmailValideur
        FROM validations v
        JOIN utilisateurs u ON v.valideurid = u.id
        WHERE v.id = @Id;
        ";

        return await _db.QueryFirstOrDefaultAsync<ValidationDto>(sql, new { Id = id });
    }

    public async Task<ValidationDto?> GetByValideurAndDemandeAsync(int valideurId, int demandeId)
    {
        var sql = @"
            SELECT v.id, v.demandecongeid, v.statut, v.commentaire, v.datevalidation, v.ordre_validation,
                   u.id as ValideurId, u.nom as NomValideur, u.email as EmailValideur
            FROM validations v
            JOIN utilisateurs u ON v.valideurid = u.id
            WHERE v.valideurid = @ValideurId AND v.demandecongeid = @DemandeId
            AND v.statut = 'En attente';
    ";
        return await _db.QueryFirstOrDefaultAsync<ValidationDto>(sql, new { ValideurId = valideurId, DemandeId = demandeId });
    }

    public async Task<int> CreateAsync(Validation validation)
    {
        var sql = @"
            INSERT INTO validations (demandecongeid, valideurid, statut, commentaire, datevalidation, ordre_validation)
            VALUES (@DemandeCongeId, @ValideurId, @Statut, @Commentaire, @DateValidation, @OrdreValidation)
            RETURNING id;
            ";

        return await _db.ExecuteScalarAsync<int>(sql, validation);
    }

    public async Task<bool> UpdateAsync(Validation validation)
    {
        var sql = @"
        UPDATE validations
        SET statut = @Statut,
            commentaire = @Commentaire,
            datevalidation = @DateValidation
        WHERE id = @Id;
        ";

        return await _db.ExecuteAsync(sql, validation) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "DELETE FROM validations WHERE id = @Id;";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
