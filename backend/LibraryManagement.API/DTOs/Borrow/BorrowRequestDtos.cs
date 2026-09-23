namespace LibraryManagement.API.DTOs.Borrow;

public class CreateBorrowRequestRequest
{
    public string BookId { get; set; } = string.Empty;
}

public class RejectBorrowRequestRequest
{
    public string? Note { get; set; }
}

public class BorrowRequestResponse
{
    public string Id { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public string BookCode { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string MemberFullName { get; set; } = string.Empty;
    public string MemberPhoneNumber { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public string? AdminNote { get; set; }
    public string? ResultingBorrowId { get; set; }
}
