using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;

    public BookService(IBookRepository bookRepository, ICategoryRepository categoryRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Book>> GetAllAsync() => await _bookRepository.GetAllAsync();

    public async Task<List<Book>> SearchAsync(string? keyword) =>
        string.IsNullOrWhiteSpace(keyword)
            ? await _bookRepository.GetAllAsync()
            : await _bookRepository.SearchAsync(keyword.Trim());

    public async Task<Book> GetByIdAsync(string id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
            throw new NotFoundException("Không tìm thấy sách.");
        return book;
    }

    public async Task<Book> CreateAsync(CreateBookModel model)
    {
        BookValidator.ValidateCreate(model);

        if (await _bookRepository.ExistsByCodeAsync(model.BookCode))
            throw new ConflictException($"Mã sách '{model.BookCode}' đã tồn tại.");

        await EnsureCategoryExistsAsync(model.CategoryId);

        var book = new Book
        {
            BookCode = model.BookCode.Trim().ToUpperInvariant(),
            Title = model.Title.Trim(),
            Author = model.Author.Trim(),
            Publisher = model.Publisher.Trim(),
            PublicationYear = model.PublicationYear,
            Introduction = model.Introduction?.Trim() ?? string.Empty,
            Summary = model.Summary?.Trim() ?? string.Empty,
            TotalCopies = model.TotalCopies,
            CategoryId = model.CategoryId,
            Status = model.TotalCopies > 0 ? BookStatus.Available : BookStatus.Unavailable,
            CreatedAt = DateTime.UtcNow
        };

        await _bookRepository.CreateAsync(book);
        return book;
    }

    public async Task<Book> UpdateAsync(string id, UpdateBookModel model)
    {
        BookValidator.ValidateUpdate(model);

        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
            throw new NotFoundException("Không tìm thấy sách.");

        await EnsureCategoryExistsAsync(model.CategoryId);

        book.Title = model.Title.Trim();
        book.Author = model.Author.Trim();
        book.Publisher = model.Publisher.Trim();
        book.PublicationYear = model.PublicationYear;
        book.Introduction = model.Introduction?.Trim() ?? string.Empty;
        book.Summary = model.Summary?.Trim() ?? string.Empty;
        book.TotalCopies = model.TotalCopies;
        book.CategoryId = model.CategoryId;
        // Sprint 2 chưa có nghiệp vụ mượn/trả nên trạng thái chỉ phụ thuộc số lượng còn lại.
        // Trạng thái "Borrowed" sẽ do BorrowService (Sprint 4) điều chỉnh.
        if (book.Status != BookStatus.Borrowed)
        {
            book.Status = model.TotalCopies > 0 ? BookStatus.Available : BookStatus.Unavailable;
        }
        book.UpdatedAt = DateTime.UtcNow;

        await _bookRepository.UpdateAsync(book);
        return book;
    }

    public async Task DeleteAsync(string id)
    {
        var deleted = await _bookRepository.DeleteAsync(id);
        if (!deleted)
            throw new NotFoundException("Không tìm thấy sách hoặc sách đã bị xóa trước đó.");
    }

    private async Task EnsureCategoryExistsAsync(string? categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId)) return;

        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null)
            throw new BusinessRuleException("Thể loại đã chọn không tồn tại.");
    }
}
