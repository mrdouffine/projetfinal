namespace GestionConge.Components.Controllers;

using GestionConge.Components.Auth;
using GestionConge.Components.DTOs;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var utilisateurs = await _service.GetAllAsync();
        return Ok(utilisateurs);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var utilisateur = await _service.GetByIdAsync(id);
        return utilisateur is not null ? Ok(utilisateur) : NotFound();
    }

    //[HttpPost("/login")]
    //public async Task<IActionResult> GetByEmailAndPassword([FromBody] UtilisateurRequestDto loginRequest)
    //{
    //    var utilisateur = await _service.GetByEmailAndPasswordAsync(loginRequest.Email, loginRequest.MotDePasse);
    //    return utilisateur is not null ? Ok(utilisateur) : Unauthorized();
    //}
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(UtilisateurAuth utilisateurAuth)
    {
        var id = await _service.CreateAsync(utilisateurAuth);
        return CreatedAtAction(nameof(GetById), new { id }, utilisateurAuth);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UtilisateurDto utilisateurDto)
    {
        if (id != utilisateurDto.Id) return BadRequest();
        var success = await _service.UpdateAsync(utilisateurDto);
        return success ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    [Authorize]
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
