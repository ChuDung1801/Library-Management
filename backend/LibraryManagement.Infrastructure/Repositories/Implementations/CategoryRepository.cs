using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly LibraryDbContext _context;

    public CategoryRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync() =>
        await _context.Categories.Find(FilterDefinition<Category>.Empty)
            .SortBy(c => c.Name)
            .ToListAsync();

    public async Task<Category?> GetByIdAsync(string id) =>
        await _context.Categories.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<bool> ExistsByNameAsync(string name) =>
        await _context.Categories.Find(c => c.Name.ToLower() == name.ToLower()).AnyAsync();

    public async Task CreateAsync(Category category) =>
        await _context.Categories.InsertOneAsync(category);
}
