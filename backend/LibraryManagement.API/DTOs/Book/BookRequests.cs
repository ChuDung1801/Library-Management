namespace LibraryManagement.API.DTOs.Book;

public class CreateBookRequest
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

public class UpdateBookRequest
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
