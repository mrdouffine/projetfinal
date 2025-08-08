namespace GestionConge.Components.Controllers;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UtilisateurController : ControllerBase
{
    private readonly IUtilisateurService _service;

    public UtilisateurController(IUtilisateurService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var utilisateurs = await _service.GetAllAsync();
        return Ok(utilisateurs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var utilisateur = await _service.GetByIdAsync(id);
        return utilisateur is not null ? Ok(utilisateur) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UtilisateurRequestDto utilisateurRequestDto)
    {
        var id = await _service.CreateAsync(utilisateurRequestDto);
        return CreatedAtAction(nameof(GetById), new { id }, utilisateurRequestDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UtilisateurDto utilisateurDto)
    {
        if (id != utilisateurDto.Id) return BadRequest();
        var success = await _service.UpdateAsync(utilisateurDto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    // Récupérer les subordonnés
    [HttpGet("{id}/subordonnes")]
    public async Task<IActionResult> GetSubordonnes(int id)
    {
        var subordonnes = await _service.GetSubordonnesAsync(id);
        return Ok(subordonnes);
    }

    //// Modifier uniquement le rôle
    //[HttpPut("{id}/role")]
    //public async Task<IActionResult> ModifierRole(int id, [FromBody] string nouveauRole)
    //{
    //    var success = await _service.ModifierRoleAsync(id, nouveauRole);
    //    return success ? NoContent() : NotFound();
    //}

}
