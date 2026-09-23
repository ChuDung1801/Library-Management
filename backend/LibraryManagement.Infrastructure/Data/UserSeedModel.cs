using System.Text.Json.Serialization;

namespace LibraryManagement.Infrastructure.Data;

/// <summary>
/// Map trực tiếp cấu trúc trong users.json (khóa tiếng Việt) sang model C#
/// để nạp seed data ban đầu vào MongoDB.
/// </summary>
public class UserSeedModel
{
    [JsonPropertyName("tên")]
    public string Ten { get; set; } = string.Empty;

    [JsonPropertyName("tênĐăngNhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [JsonPropertyName("mậtKhẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [JsonPropertyName("ngàySinh")]
    public string NgaySinh { get; set; } = string.Empty;

    [JsonPropertyName("quyềnHạn")]
    public string QuyenHan { get; set; } = string.Empty; // "Admin" | "Employee" | "User"
}
