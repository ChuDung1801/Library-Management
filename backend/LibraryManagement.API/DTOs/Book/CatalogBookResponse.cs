namespace LibraryManagement.API.DTOs.Book;

/// <summary>
/// DTO công khai cho Member/Guest xem danh sách & chi tiết sách (SCRUM-38..41).
/// Khác BookResponse (dành cho Admin) ở chỗ có sẵn CategoryName (đỡ phải gọi thêm API)
/// và không lộ CreatedAt/UpdatedAt nội bộ.
/// </summary>
public class CatalogBookResponse
{
    public string Id { get; set; } = string.Empty;
    public string BookCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Introduction { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    /// <summary>Tình trạng sách (SCRUM-40): Available | Borrowed | Unavailable.</summary>
    public string Status { get; set; } = string.Empty;
}
