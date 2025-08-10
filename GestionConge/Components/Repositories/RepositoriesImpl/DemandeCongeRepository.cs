using GestionConge.Components.Repositories.IRepositories;
namespace GestionConge.Components.Repositories.RepositoriesImpl;

using Dapper;
using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;
using System.Data;

public class DemandeCongeRepository : IDemandeCongeRepository
{
    private readonly IDbConnection _db;

    public DemandeCongeRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DemandeCongeDto>> GetAllAsync()
    {
        var sql = @"
        SELECT d.id, d.date_debut, d.date_fin, d.motif, d.statut, d.date_soumission,
               u.id as UtilisateurId, u.nom as NomUtilisateur, u.email as EmailUtilisateur,
               v.statut as StatutValidation, v.commentaire as CommentaireValidation, v.datevalidation
        FROM demandes_conge d
        JOIN utilisateurs u ON d.utilisateurid = u.id
        LEFT JOIN validations v ON v.demandecongeid = d.id
        ORDER BY d.date_soumission DESC;
        ";

        return await _db.QueryAsync<DemandeCongeDto>(sql);
    }

    public async Task<DemandeCongeDto?> GetByIdAsync(int id)
    {
        var sql = @"
        SELECT d.id, d.date_debut, d.date_fin, d.motif, d.statut, d.date_soumission,
               u.id as UtilisateurId, u.nom as NomUtilisateur, u.email as EmailUtilisateur,
               v.statut as StatutValidation, v.commentaire as CommentaireValidation, v.datevalidation
        FROM demandes_conge d
        JOIN utilisateurs u ON d.utilisateurid = u.id
        LEFT JOIN validations v ON v.demandecongeid = d.id
        WHERE d.id = @Id;
        ";

        return await _db.QueryFirstOrDefaultAsync<DemandeCongeDto>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(DemandeCongeRequestDto demande)
    {
        using var transaction = _db.BeginTransaction();

        try
        {
            // Étape 1 : Créer la demande
            var sqlInsertDemande = @"
            INSERT INTO demandes_conge (utilisateurid, date_debut, date_fin, motif, statut)
            VALUES (@UtilisateurId, @DateDebut, @DateFin, @Motif, 'En attente')
            RETURNING id;
        ";

            var demandeId = await _db.ExecuteScalarAsync<int>(sqlInsertDemande, demande, transaction);

            // Étape 2 : Chercher le supérieur hiérarchique
            var superieurId = await _db.ExecuteScalarAsync<int?>(
                "SELECT superieurid FROM utilisateurs WHERE id = @UtilisateurId",
                //"SELECT s.Id, s.Nom, s.Email FROM utilisateurs u JOIN utilisateurs s ON u.SuperieurId = s.Id WHERE u.Id = @UtilisateurId",
                new { UtilisateurId = demande.UtilisateurId }, transaction);

            //            var superieur = connection.QueryFirstOrDefault<Utilisateur>(sql, new { UtilisateurId = utilisateurId }); 

            if (superieurId is null)
            {
                throw new Exception("Aucun supérieur hiérarchique défini pour l'utilisateur.");
            }

            // Étape 3 : Créer la validation initiale
            var sqlInsertValidation = @"
            INSERT INTO validations (demandecongeid, valideurid, statut)
            VALUES (@DemandeCongeId, @ValideurId, 'En attente');
        ";

            await _db.ExecuteAsync(sqlInsertValidation, new
            {
                DemandeCongeId = demandeId,
                ValideurId = superieurId
            }, transaction);

            transaction.Commit();
            return demandeId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }


    public async Task<bool> UpdateAsync(DemandeCongeDto demande)
    {
        var sql = @"
        UPDATE demandes_conge
        SET date_debut = @DateDebut, date_fin = @DateFin, motif = @Motif, statut = @Statut
        WHERE id = @Id;
        ";

        return await _db.ExecuteAsync(sql, demande) > 0;
    }

    public async Task<bool> UpdateStatutAsync(int demandeId, string statut)
    {
        var sql = "UPDATE demande_conges SET statut = @Statut WHERE id = @DemandeId";
        return await _db.ExecuteAsync(sql, new { DemandeId = demandeId, Statut = statut }) > 0;
    }

    public async Task<IEnumerable<DemandeCongeDto>> GetByUtilisateurIdAsync(int utilisateurId)
    {
        var sql = @"
        SELECT d.id, d.date_debut, d.date_fin, d.motif, d.statut, d.date_soumission,
               u.id as UtilisateurId, u.nom as NomUtilisateur, u.email as EmailUtilisateur,
               v.statut as StatutValidation, v.commentaire as CommentaireValidation, v.datevalidation
        FROM demandes_conge d
        JOIN utilisateurs u ON d.utilisateurid = u.id
        LEFT JOIN validations v ON v.demandecongeid = d.id
        WHERE u.id = @UtilisateurId
        ORDER BY d.date_soumission DESC;
    ";

        return await _db.QueryAsync<DemandeCongeDto>(sql, new { UtilisateurId = utilisateurId });
    }

    public async Task<IEnumerable<DemandeCongeDto>> GetAssignesAsync(int valideurId)
    {
        var sql = @"
        SELECT d.id, d.date_debut, d.date_fin, d.motif, d.statut, d.date_soumission,
               u.id as UtilisateurId, u.nom as NomUtilisateur, u.email as EmailUtilisateur,
               v.statut as StatutValidation, v.commentaire as CommentaireValidation, v.datevalidation
        FROM demandes_conge d
        JOIN utilisateurs u ON d.utilisateurid = u.id
        JOIN validations v ON v.demandecongeid = d.id
        WHERE v.valideurid = @ValideurId AND d.statut = 'En attente'
        ORDER BY d.date_soumission DESC;
    ";

        return await _db.QueryAsync<DemandeCongeDto>(sql, new { ValideurId = valideurId });
    }


    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "DELETE FROM demandes_conge WHERE id = @Id;";
        return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
