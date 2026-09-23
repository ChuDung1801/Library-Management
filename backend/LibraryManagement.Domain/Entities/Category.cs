using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Thể loại sách (SCRUM-35). Một thể loại có nhiều sách (1-n với Book.CategoryId).
/// </summary>
public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tiền tố mã sách gợi ý cho thể loại này, ví dụ thể loại "Vũ trụ" → "UNIVER".
    /// Không bắt buộc; dùng để admin tham khảo khi đặt Mã sách thủ công.
    /// </summary>
    [BsonElement("codePrefix")]
    public string? CodePrefix { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
