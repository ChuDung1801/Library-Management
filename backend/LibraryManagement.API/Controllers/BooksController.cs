using LibraryManagement.API.DTOs.Book;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Quản lý sách - Epic 02 (SCRUM-19, 33, 34, 36).
/// Sprint 2 giới hạn toàn bộ endpoint cho Admin/Employee; tìm kiếm/xem công khai cho
/// Member và Guest sẽ được bổ sung ở Sprint 3 (SCRUM-37..41) qua controller/route riêng
/// để không phá vỡ giới hạn quyền đã có ở đây.
/// </summary>
[ApiController]
[Route("api/books")]
[Authorize(Roles = "Admin,Employee")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Xem danh sách tất cả sách (SCRUM-36), hoặc tìm kiếm theo tên/tác giả/mã sách
    /// khi truyền query "keyword" (SCRUM-37).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BookResponse>>> GetAll([FromQuery] string? keyword)
    {
        var books = await _bookService.SearchAsync(keyword);
        return Ok(books.Select(Map).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponse>> GetById(string id)
    {
        var book = await _bookService.GetByIdAsync(id);
        return Ok(Map(book));
    }

    /// <summary>Thêm sách mới (SCRUM-19).</summary>
    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest request)
    {
        var book = await _bookService.CreateAsync(new CreateBookModel
        {
            BookCode = request.BookCode,
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            PublicationYear = request.PublicationYear,
            Introduction = request.Introduction,
            Summary = request.Summary,
            TotalCopies = request.TotalCopies,
            CategoryId = request.CategoryId
        });

        return CreatedAtAction(nameof(GetById), new { id = book.Id }, Map(book));
    }

    /// <summary>Cập nhật thông tin sách (SCRUM-33).</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponse>> Update(string id, [FromBody] UpdateBookRequest request)
    {
        var book = await _bookService.UpdateAsync(id, new UpdateBookModel
        {
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            PublicationYear = request.PublicationYear,
            Introduction = request.Introduction,
            Summary = request.Summary,
            TotalCopies = request.TotalCopies,
            CategoryId = request.CategoryId
        });

        return Ok(Map(book));
    }

    /// <summary>Xóa sách (SCRUM-34).</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _bookService.DeleteAsync(id);
        return NoContent();
    }

    private static BookResponse Map(Domain.Entities.Book book) => new()
    {
        Id = book.Id,
        BookCode = book.BookCode,
        Title = book.Title,
        Author = book.Author,
        Publisher = book.Publisher,
        PublicationYear = book.PublicationYear,
        Introduction = book.Introduction,
        Summary = book.Summary,
        TotalCopies = book.TotalCopies,
        CategoryId = book.CategoryId,
        Status = book.Status.ToString(),
        CreatedAt = book.CreatedAt,
        UpdatedAt = book.UpdatedAt
    };
}
