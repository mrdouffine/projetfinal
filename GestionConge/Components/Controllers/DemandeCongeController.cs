using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GestionConge.Components.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DemandeCongeController : ControllerBase
{
    private readonly IDemandeCongeService _service;

    public DemandeCongeController(IDemandeCongeService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var demande = await _service.GetByIdAsync(id);
        return demande is not null ? Ok(demande) : NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DemandeCongeRequestDto demande)
    {
        var id = await _service.CreateAsync(demande);
        return CreatedAtAction(nameof(GetById), new { id }, demande);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DemandeCongeDto demande)
    {
        if (id != demande.Id) return BadRequest();
        var success = await _service.UpdateAsync(demande);
        return success ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpGet("utilisateur/{utilisateurId}")]
    public async Task<IActionResult> GetByUtilisateur(int utilisateurId)
    {
        var demandes = await _service.GetByUtilisateurIdAsync(utilisateurId);
        return Ok(demandes);
    }

    [Authorize(Roles = "DOT,Admin")]
    [HttpGet("assignes/{validateurId}")]
    public async Task<IActionResult> GetAssignes(int validateurId)
    {
        var demandes = await _service.GetAssignesAsync(validateurId);
        return Ok(demandes);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
