using backend.Model;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

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

    List<User> users = new List<User> {
        new User(
            "admin@gmail.com",
            ["users.list", "users.create", "metrics.list"],
            "123456",
            ["administrator"]
        ),
    };

    [HttpGet]
    [Route("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(users.ElementAt(0));
    }

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
        var jwtKey = _configuration["Jwt:Key"];
        var key = Encoding.ASCII.GetBytes(jwtKey!);

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
            Expires = DateTime.UtcNow.AddSeconds(10),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        Guid refreshToken = Guid.NewGuid();
        string refreshTokenString = refreshToken.ToString();

        return refreshTokenString;
    }
}