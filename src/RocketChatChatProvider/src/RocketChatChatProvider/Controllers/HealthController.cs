using Microsoft.AspNetCore.Mvc;

namespace RocketChatChatProvider.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class HealthController: ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok();
    }
}