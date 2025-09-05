namespace GestionConge.Components.Controllers;

using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly IValidationService _service;

    public ValidationController(IValidationService service)
    {
        _service = service;
    }

    //[Authorize(Roles ="Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    //[Authorize(Roles ="DOT")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var validation = await _service.GetByIdAsync(id);
        return validation is not null ? Ok(validation) : NotFound();
    }

    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Validation validation)
    {
        var id = await _service.CreateAsync(validation);
        return CreatedAtAction(nameof(GetById), new { id }, validation);
    }

    //[Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Validation validation)
    {
        if (id != validation.Id) return BadRequest();
        var success = await _service.UpdateAsync(validation);
        return success ? NoContent() : NotFound();
    }

    //[Authorize]
    [HttpPost("traiter")]
    public async Task<IActionResult> TraiterValidation([FromBody] ValidationRequestDto request)
    {
        var success = await _service.TraiterValidationAsync(request);
        return success ? Ok(new { message = "Validation traitée." }) : BadRequest("Erreur lors du traitement.");
    }

    //[Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    //GetValidationsAujourdhuiAsync
    //[Authorize]
    [HttpGet("aujourdhui")]
    public async Task<IActionResult> GetValidationsAujourdhui(int id)
    {
        var validations = await _service.GetValidationsAujourdhuiAsync(id);
        return Ok(validations);
    }

    //GetValidationsByValidateurAsync
    //[Authorize]
    [HttpGet("validations-by-validateur")]
    public async Task<IActionResult> GetValidationsByValidateur(int id)
    {
        var validations = await _service.GetValidationsByValidateurAsync(id);
        return Ok(validations);
    }

    //GetDotIdsAsync
    //[Authorize(Roles ="Admin")]
    [HttpGet("dot-ids")]
    public async Task<IActionResult> GetDotIds()
    {
        var dotIds = await _service.GetDotIdsAsync();
        return Ok(dotIds);
    }
}
