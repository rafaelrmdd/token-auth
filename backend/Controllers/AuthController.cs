using backend.Model;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace backend.Controllers;

[ApiController]
[Route("api/", Name = "Authentication")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    User[] users = [new User(
        "admin@gmail.com",
        ["users.list", "users.create", "metrics.list"],
        "123456",
        ["administrator"]
    )];

    [HttpPost]
    [Route("session")]
    public IActionResult Session(User user)
    {
        if (!(user.Email == users[0].Email && user.Password == users[0].Password))
        {
            return StatusCode(500, new { message = "Email or password is incorrect!" });
        }

        var jwt = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return Ok(new
        {
            jwt,
            refreshToken,
            users[0].Permissions,
            users[0].Roles
        });
    }

    [HttpPost]
    [Route("refresh")]
    public IActionResult Refresh(User user)
    {
        if (!(user.Email == users[0].Email && user.Password == users[0].Password))
        {
            return StatusCode(404, new { message = "Email or password is incorrect!" });
        }

        var jwt = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return Ok(new
        {
            jwt,
            refreshToken,
            users[0].Permissions,
            users[0].Roles
        });
    }

    private string GenerateJwtToken(User user)
    {
        //If jwt:key value from configurations is null, then use RosaCachorroGato
        var jwtKey = _configuration["Jwt:Key"] ?? "Roxo e legal Macarrao e bom Macarrao e Roxo e Carro Bom";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in user.Permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3), // Token válido por 3 horas
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _configuration["Jwt:Issuer"] ?? "YourAppIssuer",
            Audience = _configuration["Jwt:Audience"] ?? "YourAppAudience"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}