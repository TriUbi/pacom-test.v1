using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DeviceStatusApi.Services;

/// <summary>
/// Skapar och validerar JWT tokens
/// </summary>
public class TokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly int _expireMinutes;

    /// <summary>
    /// Hämtar hemlig nyckel och giltighetstid från inställningar
    /// </summary>
    /// <param name="config">Konfiguration med nyckel och tid</param>
    public TokenService(IConfiguration config)
    {
        var secret = config["Jwt:Key"] ?? throw new Exception("JWT Key not found.");
        _expireMinutes = int.Parse(config["Jwt:ExpireMinutes"] ?? "30");

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    /// <summary>
    /// Skapar en JWT token för den angivna användaren
    /// </summary>
    /// <param name="username">Användarnamn</param>
    /// <returns>JWT token som sträng</returns>
    public string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, username)
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Returnerar inställningar som behövs för att validera en token
    /// </summary>
    /// <returns>TokenValidationParameters</returns>
    public TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
}
