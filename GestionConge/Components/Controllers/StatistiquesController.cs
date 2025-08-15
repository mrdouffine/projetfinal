using Dapper;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace GestionConge.Components.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatistiquesController : ControllerBase
{
    private readonly IDbConnection _db;
    private readonly IPdfExportService _pdfExportService;

    public StatistiquesController(IConfiguration config, IPdfExportService pdfExportService)
    {
        _db = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        _pdfExportService = pdfExportService;
    }

    // On ajoutera les endpoints ici
    [Authorize(Roles = "Admin")]

    [HttpGet("jours-par-user")]
    public async Task<IActionResult> GetJoursParUser()
    {
        var sql = @"
    SELECT 
        u.Nom, 
        u.Email, 
        SUM(DATE_PART('day', c.Date_Fin - c.Date_Debut) + 1) AS TotalJours
    FROM Utilisateurs u
    JOIN Demandes_Conge c ON u.Id = c.UtilisateurId
    WHERE c.Statut = 'Validé'
    GROUP BY u.Id, u.Nom, u.Email
    ORDER BY TotalJours DESC";

        var result = await _db.QueryAsync(sql);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]

    [HttpGet("pic-absences")]
    public async Task<IActionResult> GetPicAbsences()
    {
        var sql = @"
        SELECT c.Date_Debut, COUNT(*) AS NombreAbsences
        FROM Demandes_Conge c
        WHERE c.Statut = 'Validé'
        GROUP BY c.Date_Debut
        ORDER BY NombreAbsences DESC
        LIMIT 5";

        var result = await _db.QueryAsync(sql);
        return Ok(result);
    }
    [Authorize(Roles = "Admin")]

    [HttpGet("solde/{utilisateurId}")]
    public async Task<IActionResult> GetSolde(int utilisateurId)
    {
        // Supposons que chaque user a droit à 30 jours/an
        var totalCongeParAn = 30;

        var sql = @"
        SELECT COALESCE(SUM(DATE_PART('day', c.Date_Fin - c.Date_Debut) + 1), 0) 
        FROM Demandes_Conge c
        WHERE UtilisateurId = @utilisateurId AND Statut = 'Validé'";

        var totalPris = await _db.ExecuteScalarAsync<int>(sql, new { utilisateurId });
        var soldeRestant = totalCongeParAn - totalPris;

        return Ok(new { SoldeRestant = soldeRestant });
    }

    [Authorize(Roles = "Admin")]

    [HttpGet("conges-par-mois")]
    public async Task<IActionResult> GetCongesParMois()
    {
        var sql = @"
        SELECT DATE_TRUNC('month', Date_Debut) AS Mois, COUNT(*) AS NombreConges
        FROM Demandes_Conge
        WHERE EXTRACT(YEAR FROM Date_Debut) = EXTRACT(YEAR FROM CURRENT_DATE)
              AND Statut = 'Validé'
        GROUP BY Mois
        ORDER BY Mois";

        var result = await _db.QueryAsync(sql);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]

    [HttpGet("export-pdf")]
    public IActionResult ExportStatistiquesEnPdf()
    {
        var pdfBytes = _pdfExportService.ExportStatistiquesEnPdf();
        return File(pdfBytes, "application/pdf", "statistiques.pdf");
    }




}
