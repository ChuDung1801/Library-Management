using System.Text.Json.Serialization;

namespace LibraryManagement.Infrastructure.Data;

/// <summary>Map trực tiếp cấu trúc trong books.json (khóa tiếng Việt) sang model C#.</summary>
public class BookSeedModel
{
    [JsonPropertyName("tênSách")]
    public string TenSach { get; set; } = string.Empty;

    [JsonPropertyName("nămXuấtBản")]
    public int NamXuatBan { get; set; }

    [JsonPropertyName("tácGiả")]
    public string TacGia { get; set; } = string.Empty;

    [JsonPropertyName("giớiThiệuSách")]
    public string GioiThieuSach { get; set; } = string.Empty;

    [JsonPropertyName("sơLượcNộiDung")]
    public string SoLuocNoiDung { get; set; } = string.Empty;
}
