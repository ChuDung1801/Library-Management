using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using LibraryManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace LibraryManagement.API.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(LibraryDbContext context) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await context.Users.Find(existing => existing.Username == request.Username && existing.IsActive).FirstOrDefaultAsync();
        if (user is null || !PasswordMatches(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(new { user.Id, user.FullName, user.Username, user.Email, user.Role, user.IsActive });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Full name, username, password, phone number and email are required.");

        if (await context.Users.Find(user => user.Username == request.Username || user.Email == request.Email).AnyAsync())
            return Conflict(new { message = "Username is already registered." });

        var user = new LibraryManagement.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Username = request.Username.Trim(),
            PasswordHash = HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            Role = LibraryManagement.Domain.Enums.Role.Member,
            IsActive = true
        };
        await context.Users.InsertOneAsync(user);
        var member = new LibraryManagement.Domain.Entities.Member
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            IsActive = true
        };
        await context.Members.InsertOneAsync(member);
        return Created($"/api/members/{member.Id}", new { user.Id, member.Id, user.FullName, user.Username, user.Email, user.Role, user.IsActive });
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"100000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool PasswordMatches(string password, string storedHash)
    {
        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations)) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

public sealed record LoginRequest(string Username, string Password);
public sealed record RegisterRequest(string FullName, string Username, string Password, string PhoneNumber, string Email);