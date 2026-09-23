using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBorrowRequestRepository
{
    Task<List<BorrowRequest>> GetAllAsync();
    Task<BorrowRequest?> GetByIdAsync(string id);
    Task<List<BorrowRequest>> GetByStatusAsync(BorrowRequestStatus status);
    Task<List<BorrowRequest>> GetByMemberIdAsync(string memberId);
    Task<bool> HasPendingRequestAsync(string memberId, string bookId);
    Task CreateAsync(BorrowRequest request);
    Task UpdateAsync(BorrowRequest request);
}
