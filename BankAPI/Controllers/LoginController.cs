using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[ApiController]
[Route("/login")]
public class LoginController : ControllerBase
{
    private readonly LoginService _service;
    private readonly ClientService _clientService;
    private IConfiguration config;

    public LoginController(LoginService service, ClientService clientService, IConfiguration iConfig)
    {
        _service = service;
        _clientService = clientService;
        config = iConfig;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Login(AdminDto adminDto)
    {
        var admin = await _service.GetAdmin(adminDto);

        if (admin is null)
        {
            return BadRequest(new { message = "Usuario no existe" });
        }

        byte[] storedHash = Convert.FromBase64String(admin.HashString);
        byte[] storedSalt = Convert.FromBase64String(admin.SaltString);
        if (!ValidatePassword(adminDto.Pwd, storedHash, storedSalt))
        {
            return BadRequest(new { message = "Credenciales invalidas" });
        }

        //generar token
        return Ok(new { token = this.GenerateToken(admin) });
    }

    [HttpPost("authenticate_client")]
    public async Task<IActionResult> ClientLogin(AuthenticateClientDTO clientToAuthDTO)
    {
        var client = await _clientService.GetById(clientToAuthDTO.Id);

        if (client is null)
        {
            return BadRequest(new { message = "Cliente no existe" });
        }

        byte[] storedHash = Convert.FromBase64String(client.HashString);
        byte[] storedSalt = Convert.FromBase64String(client.SaltString);
        if (!ValidatePassword(clientToAuthDTO.Pwd, storedHash, storedSalt))
        {
            return BadRequest(new { message = "Credenciales invalidas" });
        }

        //generar token
        return Ok(new { token = this.GenerateTokenForClient(client) });
    }

    // use this to create an admin
    [Authorize] // comment this to create an admin if there is none
    [HttpPost("createAdmin")]
    public async Task<IActionResult> CreateAdmin(AdminDtoIn adminDto)
    {
        Administrator administrator = new Administrator();
        administrator.Name = adminDto.Name;
        administrator.PhoneNumber = adminDto.PhoneNumber;
        administrator.Email = adminDto.Email;
        administrator.AdminType = adminDto.AdminType;

        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        administrator.SaltString = Convert.ToBase64String(salt);

        administrator.HashString = Convert.ToBase64String(HashPassword(salt, adminDto.Pwd));
        var newAdmin = await _service.Create(administrator);

        return Ok();
    }

    [NonAction]
    private static string BuildToken(Claim[] claims, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    [NonAction]
    private string GenerateToken(Administrator admin)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType", admin.AdminType),
        };
        return BuildToken(claims, config);
    }

    [NonAction]
    private string GenerateTokenForClient(Client client)
    {
        var claims = new[]
        {
            new Claim("Id", client.Id.ToString()),
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email, client.Email),
            new Claim("PhoneNumber", client.PhoneNumber),
        };
       return BuildToken(claims, config);
    }

    [NonAction]
    public static byte[] HashPassword(byte[] salt, string password)
    {
        return KeyDerivation.Pbkdf2(
            password: password!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: (256 / 8)
        );
    }

    [NonAction]
    private static bool ValidatePassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        byte[] hashToCompare = HashPassword(storedSalt, password);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, storedHash);
    }

    [NonAction]
    public static int GetClientID(HttpContext context)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        int possiblyIdVal;
        if (identity != null)
        {
            return int.Parse(identity.FindFirst("Id").Value);
        }
        else
        {
            return 0;
        }
    }
}