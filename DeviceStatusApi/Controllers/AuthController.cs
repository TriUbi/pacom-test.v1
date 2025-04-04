using DeviceStatusApi.Models;
using DeviceStatusApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        if (user.Username == "admin" && user.Password == "1234")
        {
            var token = _tokenService.GenerateToken(user.Username);
            return Ok(new { token });
        }

        return Unauthorized("Felaktiga inloggningsuppgifter.");
    }
}
