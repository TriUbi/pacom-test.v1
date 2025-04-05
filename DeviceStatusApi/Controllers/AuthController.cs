using DeviceStatusApi.Models;
using DeviceStatusApi.Services;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Denna controller hanterar inloggning och JWT-token
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

     /// <summary>
    /// Skapar ett nytt AuthController-objekt och kopplar in TokenService
    /// </summary>
    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Inloggning -> Om användarnamn och lösenord är rätt returneras en JWT-token
    /// </summary>
    /// <param name="user">Användarnamn + lösenord från frontend</param>
    /// <returns>En JWT-token om uppgifterna är korrekta, annars felmeddelande</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
         // Hårdkodad inloggning (user = admin / lösernord = 1234)
        if (user.Username == "admin" && user.Password == "1234")
        {
            var token = _tokenService.GenerateToken(user.Username);
            return Ok(new { token });
        }

        return Unauthorized("Felaktiga inloggningsuppgifter!");
    }
}
