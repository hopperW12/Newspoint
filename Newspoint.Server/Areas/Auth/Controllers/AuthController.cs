using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Auth.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Auth.Controllers;

[ApiController]
[Area("auth")]
[Route("api/[area]")]
public class AuthController : ControllerBase
{
    private const int ExpiresTime = 8;

    private readonly string _jwtSecret;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AuthController(
        IUserService userService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _jwtSecret = configuration["Auth:Secret"] ?? "739f17f4b258de7e39184e84bcea413b";
        _mapper = mapper;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        // Vyhledání uživatele podle e-mailu a kontrola hesla.
        var user = await _userService.GetByEmail(userLoginDto.Email);
        if (user == null || user.Password != PasswordHasher.HashPassword(userLoginDto.Password))
            return Unauthorized();

        // Při úspěšném přihlášení vytvoříme JWT token.
        var token = CreateToken(user);
        return Ok(new { Token = token });

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        var newUser = _mapper.Map<User>(userRegisterDto);

        var result = await _userService.Add(newUser);
        if (!result.Success)
            return this.ToActionResult(result);

        // Po úspěšné registraci rovnou vracíme token.
        var token = CreateToken(newUser);
        return Ok(new { Token = token });

    }

    private string CreateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        // Sestavení tokenu s identitou uživatele a rolí.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            Expires = DateTime.UtcNow.AddHours(ExpiresTime),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
