using System.Text.RegularExpressions;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Application.Validators;

/// <summary>
/// Kiểm tra dữ liệu đầu vào cho các chức năng Account Management.
/// Giữ đơn giản bằng validation thủ công thay vì thêm thư viện ngoài,
/// đúng nguyên tắc "hạn chế phụ thuộc không cần thiết" trong SKILL_LM.md.
/// </summary>
public static class AuthValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^[0-9]{9,11}$", RegexOptions.Compiled);

    public static void ValidateRegister(RegisterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.FullName) || model.FullName.Length > 50)
            throw new BusinessRuleException("Tên phải có độ dài từ 1 đến 50 ký tự.");

        if (string.IsNullOrWhiteSpace(model.Email) || !EmailRegex.IsMatch(model.Email))
            throw new BusinessRuleException("Email không hợp lệ.");

        if (string.IsNullOrWhiteSpace(model.PhoneNumber) || !PhoneRegex.IsMatch(model.PhoneNumber))
            throw new BusinessRuleException("Số điện thoại không hợp lệ (9-11 chữ số).");

        if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
            throw new BusinessRuleException("Mật khẩu phải có ít nhất 6 ký tự.");

        if (!string.IsNullOrWhiteSpace(model.Username) && model.Username.Length < 3)
            throw new BusinessRuleException("Tên đăng nhập phải có ít nhất 3 ký tự.");
    }

    public static void ValidateLogin(string usernameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            throw new BusinessRuleException("Vui lòng nhập đầy đủ tên đăng nhập/email và mật khẩu.");
    }

    public static void ValidateUpdateProfile(UpdateProfileModel model)
    {
        if (string.IsNullOrWhiteSpace(model.FullName) || model.FullName.Length > 50)
            throw new BusinessRuleException("Tên phải có độ dài từ 1 đến 50 ký tự.");

        if (string.IsNullOrWhiteSpace(model.Email) || !EmailRegex.IsMatch(model.Email))
            throw new BusinessRuleException("Email không hợp lệ.");

        if (string.IsNullOrWhiteSpace(model.PhoneNumber) || !PhoneRegex.IsMatch(model.PhoneNumber))
            throw new BusinessRuleException("Số điện thoại không hợp lệ (9-11 chữ số).");
    }
}
