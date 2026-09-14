namespace LibraryManagement.Domain.Entities;
public class Book { 
    public Guid Id { get; set; } 
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty; 
    public string Author { get; set; } = string.Empty; 
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<Guid> CategoryIds { get; set; } = [];
    public int TotalCopies { get; set; } 
    public int AvailableCopies { get; set; } 
}