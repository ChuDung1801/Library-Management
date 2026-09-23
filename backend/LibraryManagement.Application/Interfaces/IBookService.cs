using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public class CreateBookModel
{
    public string BookCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Introduction { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public string? CategoryId { get; set; }
}

public class UpdateBookModel
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Introduction { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public string? CategoryId { get; set; }
}

public interface IBookService
{
    /// <summary>Xem danh sách tất cả sách (SCRUM-36).</summary>
    Task<List<Book>> GetAllAsync();

    /// <summary>
    /// Tìm kiếm sách theo tên/tác giả/mã sách. Dùng chung cho:
    /// - Quản trị viên tìm kiếm sách (SCRUM-37, qua BooksController)
    /// - Thành viên tìm kiếm sách (SCRUM-38, qua CatalogController công khai)
    /// keyword rỗng/null → trả về toàn bộ danh sách (tương đương GetAllAsync).
    /// </summary>
    Task<List<Book>> SearchAsync(string? keyword);

    Task<Book> GetByIdAsync(string id);

    /// <summary>Thêm sách mới (SCRUM-19).</summary>
    Task<Book> CreateAsync(CreateBookModel model);

    /// <summary>Cập nhật thông tin sách (SCRUM-33).</summary>
    Task<Book> UpdateAsync(string id, UpdateBookModel model);

    /// <summary>Xóa sách (SCRUM-34).</summary>
    Task DeleteAsync(string id);
}
