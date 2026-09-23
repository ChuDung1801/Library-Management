using LibraryManagement.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Yêu cầu mượn sách do Thành viên tự gửi (SCRUM-49). Khác với Borrow (Sprint 4) -
/// vốn do Admin/Employee trực tiếp "ghi nhận mượn sách" - BorrowRequest cần Admin duyệt
/// mới thật sự trở thành một Borrow, giữ đúng phân quyền "chỉ Admin ghi nhận mượn/trả"
/// trong SKILL_LM.md mục 3.1/10, đồng thời cho Member chủ động đề nghị mượn.
/// </summary>
public class BorrowRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("bookId")]
    public string BookId { get; set; } = string.Empty;

    [BsonElement("bookCode")]
    public string BookCode { get; set; } = string.Empty;

    [BsonElement("bookTitle")]
    public string BookTitle { get; set; } = string.Empty;

    [BsonElement("memberId")]
    public string MemberId { get; set; } = string.Empty;

    [BsonElement("memberFullName")]
    public string MemberFullName { get; set; } = string.Empty;

    [BsonElement("memberPhoneNumber")]
    public string MemberPhoneNumber { get; set; } = string.Empty;

    [BsonElement("requestedAt")]
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;

    [BsonElement("processedAt")]
    public DateTime? ProcessedAt { get; set; }

    /// <summary>Lý do từ chối (nếu có) - admin điền khi Reject.</summary>
    [BsonElement("adminNote")]
    public string? AdminNote { get; set; }

    /// <summary>Id của Borrow được tạo ra khi yêu cầu này được duyệt (null nếu chưa duyệt).</summary>
    [BsonElement("resultingBorrowId")]
    public string? ResultingBorrowId { get; set; }
}
