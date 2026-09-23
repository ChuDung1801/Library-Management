namespace LibraryManagement.API.DTOs.Category;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? CodePrefix { get; set; }
    public string? Description { get; set; }
}

public class CategoryResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CodePrefix { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
