namespace LibraryManagement.API.DTOs.Borrow;

public class CreateBorrowRequest
{
    public string BookId { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
}

public class BorrowResponse
{
    public string Id { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public string BookCode { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string MemberFullName { get; set; } = string.Empty;
    public string MemberPhoneNumber { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;

    /// <summary>True nếu chưa trả và đã qua hạn (SCRUM-45) - tính động, không lưu DB.</summary>
    public bool IsOverdue { get; set; }
}
