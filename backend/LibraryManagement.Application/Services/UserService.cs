using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("Không tìm thấy người dùng.");
        return user;
    }

    public async Task<User> UpdateProfileAsync(string userId, UpdateProfileModel model)
    {
        var user = await GetByIdAsync(userId);
        return await ApplyUpdateAsync(user, model);
    }

    public async Task<List<User>> AdminGetMembersAsync() =>
        await _userRepository.GetByRoleAsync(UserRole.Member);

    public async Task<User> AdminUpdateMemberAsync(string memberId, UpdateProfileModel model)
    {
        var member = await GetMemberOrThrowAsync(memberId);
        return await ApplyUpdateAsync(member, model);
    }

    public async Task<User> AdminSetMemberActiveAsync(string memberId, bool isActive)
    {
        var member = await GetMemberOrThrowAsync(memberId);
        member.IsActive = isActive;
        member.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(member);
        return member;
    }

    private async Task<User> GetMemberOrThrowAsync(string memberId)
    {
        var user = await _userRepository.GetByIdAsync(memberId);
        if (user is null || user.Role != UserRole.Member)
            throw new NotFoundException("Không tìm thấy tài khoản thành viên.");
        return user;
    }

    private async Task<User> ApplyUpdateAsync(User user, UpdateProfileModel model)
    {
        AuthValidator.ValidateUpdateProfile(model);

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        if (!string.Equals(normalizedEmail, user.Email, StringComparison.OrdinalIgnoreCase)
            && await _userRepository.ExistsByEmailAsync(normalizedEmail))
        {
            throw new ConflictException("Email này đã được sử dụng bởi tài khoản khác.");
        }

        user.FullName = model.FullName.Trim();
        user.PhoneNumber = model.PhoneNumber.Trim();
        user.Email = normalizedEmail;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return user;
    }
}
