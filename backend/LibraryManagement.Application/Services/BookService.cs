using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
namespace LibraryManagement.Application.Services;
public class BookService : IBookService
{
    public Task<IReadOnlyCollection<Book>> SearchAsync(string? keyword) => Task.FromResult<IReadOnlyCollection<Book>>(Array.Empty<Book>());
}