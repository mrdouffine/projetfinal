namespace GestionConge.Components.Controllers;

using GestionConge.Components.Models;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PlanningCongeController : ControllerBase
{
    private readonly IPlanningCongeService _service;

    public PlanningCongeController(IPlanningCongeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is not null ? Ok(item) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanningConge planning)
    {
        var id = await _service.CreateAsync(planning);
        return CreatedAtAction(nameof(GetById), new { id }, planning);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlanningConge planning)
    {
        if (id != planning.Id) return BadRequest();
        var success = await _service.UpdateAsync(planning);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("planifier")]
    public async Task<IActionResult> Planifier([FromBody] PlanningConge planning)
    {
        try
        {
            var success = await _service.AjouterPlanningAvecVerificationsAsync(planning);
            if (!success)
                return BadRequest(new
                {
                    message = "Impossible de planifier ces congés",
                    reasons = new[] {
                    "Chevauchement avec des congés existants",
                    "Dépassement du quota annuel (30 jours)"
                }
                });

            return Ok(new { message = "Planning créé avec succès" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("utilisateur/{utilisateurId}")]
    public async Task<IActionResult> GetPlanningParUtilisateur(int utilisateurId)
    {
        var plannings = await _service.GetByUtilisateurAsync(utilisateurId);
        return Ok(plannings);
    }

    [HttpGet("solde/{utilisateurId}/{annee}")]
    public async Task<IActionResult> GetSoldeRestant(int utilisateurId, int annee)
    {
        var totalPlanifie = await _service.CalculerTotalJoursPlanifiesAsync(utilisateurId, annee);
        var soldeRestant = 30 - totalPlanifie;

        return Ok(new
        {
            AnneeReference = annee,
            TotalPlanifie = totalPlanifie,
            SoldeRestant = Math.Max(0, soldeRestant)
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
