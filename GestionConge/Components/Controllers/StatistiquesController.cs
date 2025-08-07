using Dapper;
using GestionConge.Components.Services.IServices;
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

    [HttpGet("jours-par-user")]
    public async Task<IActionResult> GetJoursParUser()
    {
        var sql = @"
        SELECT u.Nom, u.Prenom, SUM(c.Duree) AS TotalJours
        FROM Utilisateurs u
        JOIN Conges c ON u.Id = c.UtilisateurId
        WHERE c.Statut = 'Approuvé'
        GROUP BY u.Id, u.Nom, u.Prenom
        ORDER BY TotalJours DESC";

        var result = await _db.QueryAsync(sql);
        return Ok(result);
    }

    [HttpGet("pic-absences")]
    public async Task<IActionResult> GetPicAbsences()
    {
        var sql = @"
        SELECT c.DateDebut, COUNT(*) AS NombreAbsences
        FROM Conges c
        WHERE c.Statut = 'Approuvé'
        GROUP BY c.DateDebut
        ORDER BY NombreAbsences DESC
        LIMIT 5";

        var result = await _db.QueryAsync(sql);
        return Ok(result);
    }

    [HttpGet("solde/{utilisateurId}")]
    public async Task<IActionResult> GetSolde(int utilisateurId)
    {
        // Supposons que chaque user a droit à 30 jours/an
        var totalCongeParAn = 30;

        var sql = @"
        SELECT COALESCE(SUM(Duree), 0) 
        FROM Conges
        WHERE UtilisateurId = @utilisateurId AND Statut = 'Approuvé'";

        var totalPris = await _db.ExecuteScalarAsync<int>(sql, new { utilisateurId });
        var soldeRestant = totalCongeParAn - totalPris;

        return Ok(new { SoldeRestant = soldeRestant });
    }

    [HttpGet("conges-par-mois")]
    public async Task<IActionResult> GetCongesParMois()
    {
        var sql = @"
        SELECT DATE_TRUNC('month', DateDebut) AS Mois, COUNT(*) AS NombreConges
        FROM Conges
        WHERE EXTRACT(YEAR FROM DateDebut) = EXTRACT(YEAR FROM CURRENT_DATE)
              AND Statut = 'Approuvé'
        GROUP BY Mois
        ORDER BY Mois";

        var result = await _db.QueryAsync(sql);
        return Ok(result);
    }

    [HttpGet("export-pdf")]
    public IActionResult ExportStatistiquesEnPdf()
    {
        var pdfBytes = _pdfExportService.ExportStatistiquesEnPdf();
        return File(pdfBytes, "application/pdf", "statistiques.pdf");
    }




}
