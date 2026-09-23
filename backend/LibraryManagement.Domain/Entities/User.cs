using LibraryManagement.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryManagement.Domain.Entities;

/// <summary>
/// Thực thể User - áp dụng cho cả Admin, Employee và Member.
/// Cấu trúc bám theo mục "Cấu trúc dữ liệu" trong SKILL_LM.md.
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Tên đầy đủ, tối đa 50 ký tự.</summary>
    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Tên đăng nhập - bắt buộc với Admin/Employee, tùy chọn với Member
    /// (Member có thể đăng nhập bằng email).
    /// </summary>
    [BsonElement("username")]
    public string? Username { get; set; }

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("passwordSalt")]
    public string PasswordSalt { get; set; } = string.Empty;

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("role")]
    [BsonRepresentation(BsonType.String)]
    public UserRole Role { get; set; } = UserRole.Member;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}
