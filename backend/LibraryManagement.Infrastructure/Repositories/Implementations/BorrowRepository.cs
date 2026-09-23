using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories.Implementations;

public class BorrowRepository : IBorrowRepository
{
    private readonly LibraryDbContext _context;

    public BorrowRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<Borrow>> GetAllAsync() =>
        await _context.Borrows.Find(FilterDefinition<Borrow>.Empty)
            .SortByDescending(b => b.BorrowDate)
            .ToListAsync();

    public async Task<Borrow?> GetByIdAsync(string id) =>
        await _context.Borrows.Find(b => b.Id == id).FirstOrDefaultAsync();

    public async Task<List<Borrow>> GetActiveAsync() =>
        await _context.Borrows.Find(b => b.Status == BorrowStatus.Borrowed)
            .SortBy(b => b.DueDate)
            .ToListAsync();

    public async Task<List<Borrow>> GetByMemberIdAsync(string memberId) =>
        await _context.Borrows.Find(b => b.MemberId == memberId)
            .SortByDescending(b => b.BorrowDate)
            .ToListAsync();

    public async Task<List<Borrow>> GetActiveByMemberIdAsync(string memberId) =>
        await _context.Borrows.Find(b => b.MemberId == memberId && b.Status == BorrowStatus.Borrowed)
            .SortBy(b => b.DueDate)
            .ToListAsync();

    public async Task CreateAsync(Borrow borrow) =>
        await _context.Borrows.InsertOneAsync(borrow);

    public async Task UpdateAsync(Borrow borrow) =>
        await _context.Borrows.ReplaceOneAsync(b => b.Id == borrow.Id, borrow);
}
