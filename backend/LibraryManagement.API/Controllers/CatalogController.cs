using LibraryManagement.API.DTOs.Book;
using LibraryManagement.API.DTOs.Category;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Catalog công khai - dùng cho Guest (chưa đăng nhập) và Member.
/// Không có [Authorize]: mọi request đều được phục vụ, kể cả không kèm token,
/// đúng nghiệp vụ "Khách tìm kiếm/xem sách" (SCRUM-41) và "Thành viên tìm kiếm/xem sách" (SCRUM-38..40).
/// Đây là read-only - không có endpoint tạo/sửa/xóa nào ở controller này.
/// </summary>
[ApiController]
[Route("api/catalog")]
[AllowAnonymous]
public class CatalogController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ICategoryService _categoryService;

    public CatalogController(IBookService bookService, ICategoryService categoryService)
    {
        _bookService = bookService;
        _categoryService = categoryService;
    }

    /// <summary>
    /// Xem danh sách sách (SCRUM-41 - Khách), hoặc tìm kiếm theo tên/tác giả/mã sách
    /// khi truyền query "keyword" (SCRUM-38 - Thành viên). Hỗ trợ lọc theo categoryId.
    /// </summary>
    [HttpGet("books")]
    public async Task<ActionResult<List<CatalogBookResponse>>> GetBooks(
        [FromQuery] string? keyword,
        [FromQuery] string? categoryId)
    {
        var books = await _bookService.SearchAsync(keyword);

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            books = books.Where(b => b.CategoryId == categoryId).ToList();
        }

        var categoryMap = await BuildCategoryMapAsync();
        return Ok(books.Select(b => Map(b, categoryMap)).ToList());
    }

    /// <summary>Xem chi tiết + tình trạng sách (SCRUM-39, SCRUM-40).</summary>
    [HttpGet("books/{id}")]
    public async Task<ActionResult<CatalogBookResponse>> GetBookById(string id)
    {
        var book = await _bookService.GetByIdAsync(id);
        var categoryMap = await BuildCategoryMapAsync();
        return Ok(Map(book, categoryMap));
    }

    /// <summary>Danh sách thể loại - phục vụ bộ lọc trên trang Catalog.</summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryResponse>>> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            CodePrefix = c.CodePrefix,
            Description = c.Description,
            CreatedAt = c.CreatedAt
        }).ToList());
    }

    private async Task<Dictionary<string, string>> BuildCategoryMapAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        return categories.ToDictionary(c => c.Id, c => c.Name);
    }

    private static CatalogBookResponse Map(Book book, Dictionary<string, string> categoryMap) => new()
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
        CategoryName = book.CategoryId != null && categoryMap.TryGetValue(book.CategoryId, out var name) ? name : null,
        Status = book.Status.ToString()
    };
}
