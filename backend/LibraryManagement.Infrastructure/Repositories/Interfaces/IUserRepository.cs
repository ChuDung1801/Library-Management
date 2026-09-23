using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<long> CountAsync();

    /// <summary>Lấy danh sách người dùng theo role - dùng cho SCRUM-24 (Admin quản lý thành viên).</summary>
    Task<List<User>> GetByRoleAsync(UserRole role);
}

