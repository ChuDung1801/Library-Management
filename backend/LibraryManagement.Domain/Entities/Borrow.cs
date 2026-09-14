using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Borrow
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Active;
}