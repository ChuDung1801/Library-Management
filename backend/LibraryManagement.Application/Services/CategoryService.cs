using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> CreateAsync(CreateCategoryModel model)
    {
        CategoryValidator.ValidateCreate(model);

        if (await _categoryRepository.ExistsByNameAsync(model.Name))
            throw new ConflictException($"Thể loại '{model.Name}' đã tồn tại.");

        var category = new Category
        {
            Name = model.Name.Trim(),
            CodePrefix = string.IsNullOrWhiteSpace(model.CodePrefix) ? null : model.CodePrefix.Trim().ToUpperInvariant(),
            Description = model.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.CreateAsync(category);
        return category;
    }

    public async Task<List<Category>> GetAllAsync() => await _categoryRepository.GetAllAsync();
}
