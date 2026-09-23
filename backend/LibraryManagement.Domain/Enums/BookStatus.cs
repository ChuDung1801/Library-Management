namespace LibraryManagement.Domain.Enums;

/// <summary>
/// Trạng thái sách theo mục 11 SKILL_LM.md.
/// Sprint 2 (Book Management) chỉ tính toán Available/Unavailable dựa trên số lượng còn lại.
/// Borrowed sẽ được cập nhật ở Sprint 4 (Borrow & Return) khi nghiệp vụ mượn/trả được triển khai.
/// </summary>
public enum BookStatus
{
    Available = 0,
    Borrowed = 1,
    Unavailable = 2
}
