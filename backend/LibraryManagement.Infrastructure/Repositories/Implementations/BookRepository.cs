using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories.Implementations;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllAsync() =>
        await _context.Books.Find(FilterDefinition<Book>.Empty)
            .SortByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<List<Book>> SearchAsync(string keyword)
    {
        // Tìm theo Tên sách, Tác giả hoặc Mã sách, không phân biệt hoa/thường (SCRUM-37, 38).
        var pattern = new MongoDB.Bson.BsonRegularExpression(System.Text.RegularExpressions.Regex.Escape(keyword), "i");
        var filter = Builders<Book>.Filter.Or(
            Builders<Book>.Filter.Regex(b => b.Title, pattern),
            Builders<Book>.Filter.Regex(b => b.Author, pattern),
            Builders<Book>.Filter.Regex(b => b.BookCode, pattern)
        );
        return await _context.Books.Find(filter)
            .SortByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(string id) =>
        await _context.Books.Find(b => b.Id == id).FirstOrDefaultAsync();

    public async Task<bool> ExistsByCodeAsync(string bookCode, string? excludeId = null)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.BookCode, bookCode);
        if (!string.IsNullOrEmpty(excludeId))
        {
            filter = Builders<Book>.Filter.And(filter, Builders<Book>.Filter.Ne(b => b.Id, excludeId));
        }
        return await _context.Books.Find(filter).AnyAsync();
    }

    public async Task CreateAsync(Book book) =>
        await _context.Books.InsertOneAsync(book);

    public async Task UpdateAsync(Book book) =>
        await _context.Books.ReplaceOneAsync(b => b.Id == book.Id, book);

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Books.DeleteOneAsync(b => b.Id == id);
        return result.DeletedCount > 0;
    }
}
