using System.Text.RegularExpressions;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Application.Validators;

public static class BookValidator
{
    private static readonly Regex BookCodeRegex = new(@"^[A-Z0-9]{2,15}-[0-9]{3,6}$", RegexOptions.Compiled);
    private static readonly int MinYear = 1450; // sau khi máy in ra đời, đủ rộng cho mọi sách thật
    private static readonly int MaxYear = DateTime.UtcNow.Year + 1; // cho phép sách sắp xuất bản

    public static void ValidateCreate(CreateBookModel model)
    {
        if (string.IsNullOrWhiteSpace(model.BookCode) || !BookCodeRegex.IsMatch(model.BookCode))
            throw new BusinessRuleException("Mã sách không hợp lệ. Định dạng đúng: PREFIX-0001 (vd: UNIVER-0001).");

        ValidateCommon(model.Title, model.Author, model.Publisher, model.PublicationYear, model.TotalCopies);
    }

    public static void ValidateUpdate(UpdateBookModel model) =>
        ValidateCommon(model.Title, model.Author, model.Publisher, model.PublicationYear, model.TotalCopies);

    private static void ValidateCommon(string title, string author, string publisher, int year, int totalCopies)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleException("Tên sách không được để trống.");

        if (string.IsNullOrWhiteSpace(author))
            throw new BusinessRuleException("Tác giả không được để trống.");

        if (string.IsNullOrWhiteSpace(publisher))
            throw new BusinessRuleException("Nhà xuất bản không được để trống.");

        if (year < MinYear || year > MaxYear)
            throw new BusinessRuleException($"Năm xuất bản phải trong khoảng {MinYear} - {MaxYear}.");

        if (totalCopies < 0)
            throw new BusinessRuleException("Số sách hiện có không được nhỏ hơn 0.");
    }
}
