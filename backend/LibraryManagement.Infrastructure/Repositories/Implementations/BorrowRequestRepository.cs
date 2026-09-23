using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories.Implementations;

public class BorrowRequestRepository : IBorrowRequestRepository
{
    private readonly LibraryDbContext _context;

    public BorrowRequestRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<BorrowRequest>> GetAllAsync() =>
        await _context.BorrowRequests.Find(FilterDefinition<BorrowRequest>.Empty)
            .SortByDescending(r => r.RequestedAt)
            .ToListAsync();

    public async Task<BorrowRequest?> GetByIdAsync(string id) =>
        await _context.BorrowRequests.Find(r => r.Id == id).FirstOrDefaultAsync();

    public async Task<List<BorrowRequest>> GetByStatusAsync(BorrowRequestStatus status) =>
        await _context.BorrowRequests.Find(r => r.Status == status)
            .SortBy(r => r.RequestedAt)
            .ToListAsync();

    public async Task<List<BorrowRequest>> GetByMemberIdAsync(string memberId) =>
        await _context.BorrowRequests.Find(r => r.MemberId == memberId)
            .SortByDescending(r => r.RequestedAt)
            .ToListAsync();

    public async Task<bool> HasPendingRequestAsync(string memberId, string bookId) =>
        await _context.BorrowRequests.Find(r =>
                r.MemberId == memberId && r.BookId == bookId && r.Status == BorrowRequestStatus.Pending)
            .AnyAsync();

    public async Task CreateAsync(BorrowRequest request) =>
        await _context.BorrowRequests.InsertOneAsync(request);

    public async Task UpdateAsync(BorrowRequest request) =>
        await _context.BorrowRequests.ReplaceOneAsync(r => r.Id == request.Id, request);
}
