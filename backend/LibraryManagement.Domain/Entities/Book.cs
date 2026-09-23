using LibraryManagement.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Thực thể Book - bám theo mục "Cấu trúc dữ liệu / Books" trong SKILL_LM.md:
/// Mã sách, Tên sách, Tác giả, Nhà xuất bản, Năm xuất bản, Giới thiệu sách,
/// Sơ lược nội dung, Số sách hiện có.
/// </summary>
public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Mã sách duy nhất, ví dụ "UNIVER-0001".</summary>
    [BsonElement("bookCode")]
    public string BookCode { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("author")]
    public string Author { get; set; } = string.Empty;

    [BsonElement("publisher")]
    public string Publisher { get; set; } = string.Empty;

    [BsonElement("publicationYear")]
    public int PublicationYear { get; set; }

    [BsonElement("introduction")]
    public string Introduction { get; set; } = string.Empty;

    [BsonElement("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>Số sách hiện có (tổng số bản trong thư viện).</summary>
    [BsonElement("totalCopies")]
    public int TotalCopies { get; set; }

    /// <summary>
    /// Thể loại của sách (tham chiếu Category.Id). Có thể null nếu chưa phân loại
    /// - hệ thống vẫn cho phép thêm sách trước, gán thể loại sau.
    /// </summary>
    [BsonElement("categoryId")]
    public string? CategoryId { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public BookStatus Status { get; set; } = BookStatus.Available;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
