using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBorrowRepository
{
    Task<List<Borrow>> GetAllAsync();
    Task<Borrow?> GetByIdAsync(string id);

    /// <summary>Phiếu mượn chưa trả (dùng để tính quá hạn - SCRUM-45).</summary>
    Task<List<Borrow>> GetActiveAsync();

    /// <summary>Toàn bộ phiếu mượn của một thành viên - lịch sử (SCRUM-46, 47).</summary>
    Task<List<Borrow>> GetByMemberIdAsync(string memberId);

    /// <summary>Phiếu mượn CHƯA TRẢ của một thành viên - đang mượn (SCRUM-48).</summary>
    Task<List<Borrow>> GetActiveByMemberIdAsync(string memberId);

    Task CreateAsync(Borrow borrow);
    Task UpdateAsync(Borrow borrow);
}
