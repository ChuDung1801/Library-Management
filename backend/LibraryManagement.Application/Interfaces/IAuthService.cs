using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

// Lưu ý: IPasswordHasher và IJwtTokenGenerator được định nghĩa ở tầng Infrastructure.Security
// (không phải Application) để tránh circular project reference, vì Infrastructure không được
// phép phụ thuộc ngược lại Application theo sơ đồ dependency trong FolderContruct.md.

public class RegisterModel
{
    public string FullName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class AuthResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public User User { get; set; } = null!;
}

public interface IAuthService
{
    /// <summary>
    /// Đăng ký tài khoản. Dùng chung cho "Thành viên tạo tài khoản" (SCRUM-16)
    /// và "Khách tạo tài khoản" (SCRUM-31) - cả hai đều tạo User role Member.
    /// </summary>
    Task<AuthResult> RegisterAsync(RegisterModel model);

    /// <summary>Đăng nhập bằng username hoặc email + mật khẩu (SCRUM-29).</summary>
    Task<AuthResult> LoginAsync(string usernameOrEmail, string password);
}
