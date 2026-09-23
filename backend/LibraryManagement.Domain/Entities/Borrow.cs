using LibraryManagement.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Thực thể Borrow - bám theo mục "Cấu trúc dữ liệu / Borrows" trong SKILL_LM.md:
/// Tên sách, Người mượn, Số điện thoại, Ngày mượn, Ngày phải trả.
/// Lưu thêm BookId/MemberId để tham chiếu, và snapshot Tên sách/Tên người mượn/SĐT tại
/// thời điểm mượn (đúng tinh thần dữ liệu gốc trong SKILL_LM.md, đồng thời tránh vỡ lịch sử
/// nếu sau này thành viên đổi tên/SĐT hoặc admin sửa thông tin sách).
/// </summary>
public class Borrow
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

    [BsonElement("borrowDate")]
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

    /// <summary>Hạn trả = BorrowDate + 1 tháng (quy tắc trong SKILL_LM.md mục 19).</summary>
    [BsonElement("dueDate")]
    public DateTime DueDate { get; set; }

    [BsonElement("returnDate")]
    public DateTime? ReturnDate { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
