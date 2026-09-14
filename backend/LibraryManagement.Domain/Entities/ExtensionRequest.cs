using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class ExtensionRequest
{
    public Guid Id { get; set; }
    public Guid BorrowId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? NewDueAt { get; set; }
    public ExtensionRequestStatus Status { get; set; } = ExtensionRequestStatus.Pending;
}