using LibraryManagement.Domain.Entities;
namespace LibraryManagement.Application.Validators;
public static class BookValidator { public static bool IsValid(Book book) => !string.IsNullOrWhiteSpace(book.Title) && book.TotalCopies >= 0; }