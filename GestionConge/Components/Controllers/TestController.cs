using Microsoft.AspNetCore.Mvc;
using GestionConge.Components.Repositories;

namespace GestionConge.Components.Controllers;



[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly TestRepository _testRepo;

    public TestController(TestRepository testRepo)
    {
        _testRepo = testRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _testRepo.TestConnectionAsync();
        return Ok(result);
    }
}

