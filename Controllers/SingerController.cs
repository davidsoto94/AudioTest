using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test.Database.Entities;
using test.Database.Repositories.Interfaces;

namespace test.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class SingerController(ILogger<SingerController> logger, ISingerRepository singerRepository) : ControllerBase
{
    private readonly ILogger<SingerController> _logger = logger;
    private ISingerRepository _singerRepository = singerRepository;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var singers = await _singerRepository.GetAllSingersWithNotTrackingAsync();
        return Ok(singers);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]    
    public async Task<IActionResult> Post([FromBody] Singer singer)
    {
        await _singerRepository.AddSingerWithNoTranckingAsync(singer);
        return Ok();
    }
}
