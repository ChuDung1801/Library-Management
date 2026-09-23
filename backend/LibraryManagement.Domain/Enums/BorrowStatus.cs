namespace LibraryManagement.Domain.Enums;

/// <summary>
/// Trạng thái phiếu mượn. "Quá hạn" KHÔNG phải một giá trị persisted riêng - được tính động
/// (Status == Borrowed && DueDate < now) ở tầng Application/API để tránh cần background job
/// cập nhật trạng thái theo thời gian thực.
/// </summary>
public enum BorrowStatus
{
    Borrowed = 0,
    Returned = 1
}
