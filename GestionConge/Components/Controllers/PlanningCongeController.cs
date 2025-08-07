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
        var success = await _service.AjouterPlanningAvecVerificationsAsync(planning);
        if (!success)
            return BadRequest("Chevauchement détecté ou dépassement du quota annuel (30 jours).");
        return Ok();
    }

    [HttpGet("utilisateur/{utilisateurId}")]
    public async Task<IActionResult> GetPlanningParUtilisateur(int utilisateurId)
    {
        var plannings = await _service.GetByUtilisateurAsync(utilisateurId);
        return Ok(plannings);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
