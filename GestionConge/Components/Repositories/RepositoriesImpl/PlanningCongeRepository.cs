namespace GestionConge.Components.Repositories.RepositoriesImpl
{
    using Dapper;
    using GestionConge.Components.DTOs;
    using GestionConge.Components.Models;
    using GestionConge.Components.Repositories.IRepositories;
    using System.Data;

    public class PlanningCongeRepository : IPlanningCongeRepository
    {
        private readonly IDbConnection _db;

        public PlanningCongeRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<PlanningCongeDto>> GetAllAsync()
        {
            var sql = @"
        SELECT p.id, p.date_debut, p.date_fin, p.motif, p.date_creation,
               u.id AS UtilisateurId, u.nom AS NomUtilisateur, u.email AS EmailUtilisateur
        FROM plannings_conge p
        JOIN utilisateurs u ON p.utilisateurid = u.id
        ORDER BY p.date_creation DESC;
        ";

            return await _db.QueryAsync<PlanningCongeDto>(sql);
        }

        public async Task<PlanningCongeDto?> GetByIdAsync(int id)
        {
            var sql = @"
        SELECT p.id, p.date_debut, p.date_fin, p.motif, p.date_creation,
               u.id AS UtilisateurId, u.nom AS NomUtilisateur, u.email AS EmailUtilisateur
        FROM plannings_conge p
        JOIN utilisateurs u ON p.utilisateurid = u.id
        WHERE p.id = @Id;
        ";

            return await _db.QueryFirstOrDefaultAsync<PlanningCongeDto>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(PlanningConge planning)
        {
            var sql = @"
        INSERT INTO plannings_conge (utilisateurid, date_debut, date_fin, motif, date_creation)
        VALUES (@UtilisateurId, @DateDebut, @DateFin, @Motif, @DateCreation)
        RETURNING id;
        ";

            return await _db.ExecuteScalarAsync<int>(sql, planning);
        }

        public async Task<bool> UpdateAsync(PlanningConge planning)
        {
            var sql = @"
        UPDATE plannings_conge
        SET date_debut = @DateDebut, date_fin = @DateFin, motif = @Motif
        WHERE id = @Id;
        ";

            return await _db.ExecuteAsync(sql, planning) > 0;
        }

        public async Task<bool> VerifierChevauchementAsync(int utilisateurId, DateTime debut, DateTime fin)
        {
            var sql = @"
        SELECT COUNT(1)
        FROM plannings_conge
        WHERE utilisateurid = @UtilisateurId
          AND NOT (date_fin < @DateDebut OR date_debut > @DateFin)";
            var count = await _db.ExecuteScalarAsync<int>(sql, new { UtilisateurId = utilisateurId, DateDebut = debut, DateFin = fin });
            return count > 0;
        }

        public async Task<IEnumerable<PlanningCongeDto>> GetByUtilisateurIdAsync(int utilisateurId)
        {
            var sql = @"
    SELECT p.id, p.date_debut, p.date_fin, p.motif, p.date_creation,
           u.id AS UtilisateurId, u.nom AS NomUtilisateur
    FROM plannings_conge p
    JOIN utilisateurs u ON p.utilisateurid = u.id
    WHERE p.utilisateurid = @UtilisateurId
    ORDER BY p.date_debut;
    ";
            return await _db.QueryAsync<PlanningCongeDto>(sql, new { UtilisateurId = utilisateurId });
        }
        public async Task<int> CalculerTotalJoursPlanifiesAsync(int utilisateurId, int annee)
        {
            var debutAnnee = new DateTime(annee, 1, 1);
            var finAnnee = new DateTime(annee, 12, 31);
            var sql = @"
        SELECT COALESCE(SUM(EXTRACT(DAY FROM (LEAST(date_fin, @FinAnnee) - GREATEST(date_debut, @DebutAnnee))) + 1), 0)::int
        FROM plannings_conge
        WHERE utilisateurid = @UtilisateurId
          AND date_debut <= @FinAnnee
          AND date_fin >= @DebutAnnee";
            return await _db.ExecuteScalarAsync<int>(sql, new { UtilisateurId = utilisateurId, DebutAnnee = debutAnnee, FinAnnee = finAnnee });
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM plannings_conge WHERE id = @Id;";
            return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }

}
