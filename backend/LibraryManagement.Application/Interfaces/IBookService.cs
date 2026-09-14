using LibraryManagement.Domain.Entities;
namespace LibraryManagement.Application.Interfaces;
public interface IBookService { Task<IReadOnlyCollection<Book>> SearchAsync(string? keyword); }