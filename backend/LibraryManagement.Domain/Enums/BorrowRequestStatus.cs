namespace LibraryManagement.Domain.Enums;

/// <summary>Trạng thái yêu cầu mượn sách do Thành viên gửi (SCRUM-49).</summary>
public enum BorrowRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
