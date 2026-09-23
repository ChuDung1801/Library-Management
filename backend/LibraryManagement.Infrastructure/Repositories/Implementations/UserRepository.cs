using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly LibraryDbContext _context;

    public UserRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id) =>
        await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Eq(u => u.Username, usernameOrEmail),
            Builders<User>.Filter.Eq(u => u.Email, usernameOrEmail)
        );
        return await _context.Users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _context.Users.Find(u => u.Email == email).AnyAsync();

    public async Task<bool> ExistsByUsernameAsync(string username) =>
        await _context.Users.Find(u => u.Username == username).AnyAsync();

    public async Task CreateAsync(User user) =>
        await _context.Users.InsertOneAsync(user);

    public async Task UpdateAsync(User user) =>
        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);

    public async Task<long> CountAsync() =>
        await _context.Users.CountDocumentsAsync(FilterDefinition<User>.Empty);

    public async Task<List<User>> GetByRoleAsync(UserRole role) =>
        await _context.Users.Find(u => u.Role == role)
            .SortBy(u => u.FullName)
            .ToListAsync();
}
