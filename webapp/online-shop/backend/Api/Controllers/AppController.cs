using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api")]
public class AppController: ControllerBase
{
    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        return Ok();
    }
}
