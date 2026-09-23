using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Infrastructure.Security;

namespace LibraryManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResult> RegisterAsync(RegisterModel model)
    {
        AuthValidator.ValidateRegister(model);

        if (await _userRepository.ExistsByEmailAsync(model.Email))
            throw new ConflictException("Email này đã được sử dụng để đăng ký.");

        if (!string.IsNullOrWhiteSpace(model.Username) && await _userRepository.ExistsByUsernameAsync(model.Username))
            throw new ConflictException("Tên đăng nhập này đã tồn tại.");

        var (hash, salt) = _passwordHasher.HashPassword(model.Password);

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Username = string.IsNullOrWhiteSpace(model.Username) ? null : model.Username.Trim(),
            Email = model.Email.Trim().ToLowerInvariant(),
            PhoneNumber = model.PhoneNumber.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Member, // Đăng ký công khai luôn tạo tài khoản Member (SCRUM-16 / SCRUM-31)
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResult { Token = token, ExpiresAt = expiresAt, User = user };
    }

    public async Task<AuthResult> LoginAsync(string usernameOrEmail, string password)
    {
        AuthValidator.ValidateLogin(usernameOrEmail, password);

        var user = await _userRepository.GetByUsernameOrEmailAsync(usernameOrEmail.Trim());
        if (user is null || !user.IsActive)
            throw new InvalidCredentialsException();

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidCredentialsException();

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResult { Token = token, ExpiresAt = expiresAt, User = user };
    }
}
