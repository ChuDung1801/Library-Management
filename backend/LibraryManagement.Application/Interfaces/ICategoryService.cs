using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public class CreateCategoryModel
{
    public string Name { get; set; } = string.Empty;
    public string? CodePrefix { get; set; }
    public string? Description { get; set; }
}

public interface ICategoryService
{
    /// <summary>Thêm thể loại sách mới (SCRUM-35).</summary>
    Task<Category> CreateAsync(CreateCategoryModel model);

    Task<List<Category>> GetAllAsync();
}
