using LibraryManagement.API.DTOs.Auth;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng ký tài khoản Member. Dùng chung cho "Thành viên tạo tài khoản" (SCRUM-16)
    /// và "Khách tạo tài khoản" (SCRUM-31) vì cả hai luồng nghiệp vụ đều tạo ra role Member.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(new RegisterModel
        {
            FullName = request.FullName,
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber
        });

        return Ok(MapToResponse(result));
    }

    /// <summary>Đăng nhập bằng username hoặc email (SCRUM-29).</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.UsernameOrEmail, request.Password);
        return Ok(MapToResponse(result));
    }

    private static AuthResponse MapToResponse(AuthResult result) => new()
    {
        Token = result.Token,
        ExpiresAt = result.ExpiresAt,
        User = new UserResponse
        {
            Id = result.User.Id,
            FullName = result.User.FullName,
            Username = result.User.Username,
            Email = result.User.Email,
            PhoneNumber = result.User.PhoneNumber,
            Role = result.User.Role.ToString()
        }
    };
}
