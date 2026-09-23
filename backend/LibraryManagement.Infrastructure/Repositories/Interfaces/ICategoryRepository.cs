using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(string id);
    Task<bool> ExistsByNameAsync(string name);
    Task CreateAsync(Category category);
}
