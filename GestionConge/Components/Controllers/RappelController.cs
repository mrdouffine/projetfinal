namespace GestionConge.Components.Controllers;

using GestionConge.Components.Models;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RappelController : ControllerBase
{
    private readonly IRappelService _service;

    public RappelController(IRappelService service)
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
    public async Task<IActionResult> Create([FromBody] Rappel rappel)
    {
        var id = await _service.CreateAsync(rappel);
        return CreatedAtAction(nameof(GetById), new { id }, rappel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Rappel rappel)
    {
        if (id != rappel.Id) return BadRequest();
        var success = await _service.UpdateAsync(rappel);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
