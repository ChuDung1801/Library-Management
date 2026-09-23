using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<List<Book>> SearchAsync(string keyword);
    Task<Book?> GetByIdAsync(string id);
    Task<bool> ExistsByCodeAsync(string bookCode, string? excludeId = null);
    Task CreateAsync(Book book);
    Task UpdateAsync(Book book);
    Task<bool> DeleteAsync(string id);
}
