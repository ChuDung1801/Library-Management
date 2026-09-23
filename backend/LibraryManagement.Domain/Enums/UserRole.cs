namespace LibraryManagement.Domain.Enums;

/// <summary>
/// Phân quyền hệ thống: Admin quản trị toàn bộ, Employee (nhân viên) hỗ trợ nghiệp vụ,
/// Member (khách hàng/thành viên) sử dụng chức năng mượn - trả sách.
/// </summary>
public enum UserRole
{
    Admin = 0,
    Employee = 1,
    Member = 2
}
