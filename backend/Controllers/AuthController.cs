using backend.Model;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.Abstractions;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/", Name = "Authentication")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManagerService _userManagerService;
    public AuthController(IConfiguration configuration, UserManagerService userManagerService)
    {
        _configuration = configuration;
        _userManagerService = userManagerService;
    }

    List<User> users = new List<User> {
        new User(
            "admin@gmail.com",
            ["users.list", "users.create", "metrics.list"],
            "123456",
            ["administrator"]
        ),
    };

    List<User> loggedUsers = new List<User>();

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
        User defaultUser = users[0];

        if (!(user.Email == defaultUser.Email && user.Password == defaultUser.Password))
        {
            return StatusCode(500, new { message = "Email or password is incorrect" });
        }

        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return StatusCode(500, new { message = "You should enter the credentials" });
        }

        var jwt = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var newLoggedUser = new User(
            user.Email,
            user.Permissions,
            user.Password,
            user.Roles,
            jwt,
            refreshToken
        );

        _userManagerService.AddUser(newLoggedUser);

        User? specificUser = _userManagerService.LoggedUsers.FirstOrDefault(loggedUser => loggedUser.JWT == jwt);

        if (specificUser == null)
        {
            return StatusCode(500, new { message = "User not found" });
        }

        return Ok(new
        {
            jwt,
            refreshToken,
            permissions = specificUser.Permissions,
            roles = specificUser.Roles
        });
    }

    [HttpPost]
    [Route("refresh")]
    public IActionResult Refresh(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return StatusCode(500, new { message = "Refresh token is necessary" });
        }

        var newRefreshToken = GenerateRefreshToken();

        User? user = _userManagerService.LoggedUsers.Find(user => user.RefreshToken == refreshToken);

        if (user == null)
        {
            return Unauthorized("Invalid refresh token");
        }

        var jwt = GenerateJwtToken(user);

        return Ok(new
        {
            jwt,
            newRefreshToken,
        });
    }

    [HttpGet]
    public List<User> getLoggedUsers()
    {
        return _userManagerService.GetLoggedUsers();
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