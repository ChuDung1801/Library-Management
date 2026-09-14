namespace LibraryManagement.Domain.Entities;

public class BorrowDetail
{
    public Guid Id { get; set; }
    public Guid BorrowId { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; } = 1;
}