using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public class UpdateProfileModel
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public interface IUserService
{
    Task<User> GetByIdAsync(string userId);

    /// <summary>Cập nhật thông tin cá nhân (SCRUM-30) - user tự sửa hồ sơ của chính mình.</summary>
    Task<User> UpdateProfileAsync(string userId, UpdateProfileModel model);

    /// <summary>Admin xem danh sách tài khoản thành viên (SCRUM-24).</summary>
    Task<List<User>> AdminGetMembersAsync();

    /// <summary>Admin cập nhật thông tin một thành viên (SCRUM-42).</summary>
    Task<User> AdminUpdateMemberAsync(string memberId, UpdateProfileModel model);

    /// <summary>
    /// Admin kích hoạt/vô hiệu hóa tài khoản thành viên - một phần của
    /// "quản lý tài khoản thành viên" (SCRUM-24).
    /// </summary>
    Task<User> AdminSetMemberActiveAsync(string memberId, bool isActive);
}
