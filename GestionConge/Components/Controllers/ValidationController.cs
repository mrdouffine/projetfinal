namespace GestionConge.Components.Controllers;

using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;
using GestionConge.Components.Services.IServices;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var validation = await _service.GetByIdAsync(id);
        return validation is not null ? Ok(validation) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Validation validation)
    {
        var id = await _service.CreateAsync(validation);
        return CreatedAtAction(nameof(GetById), new { id }, validation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Validation validation)
    {
        if (id != validation.Id) return BadRequest();
        var success = await _service.UpdateAsync(validation);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("traiter")]
    public async Task<IActionResult> TraiterValidation([FromBody] ValidationRequestDto request)
    {
        var success = await _service.TraiterValidationAsync(request);
        return success ? Ok(new { message = "Validation traitée." }) : BadRequest("Erreur lors du traitement.");
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
