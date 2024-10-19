using Microsoft.AspNetCore.Mvc;
using test.Database.Entities;
using test.Services;

namespace test.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthenticateController (ILogger<AuthenticateController> logger,AuthService authService) : ControllerBase
{
    private readonly ILogger<AuthenticateController> _logger = logger;

    [HttpPost]
    public IActionResult Post(User user)
    {
        try
        {
            var token = authService.GenerateToken(user);
            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500);
        }
        
    }
}
