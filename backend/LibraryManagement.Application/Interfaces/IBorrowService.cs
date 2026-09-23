using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public class CreateBorrowModel
{
    public string BookId { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
}

public interface IBorrowService
{
    Task<List<Borrow>> GetAllAsync();

    Task<Borrow> GetByIdAsync(string id);

    /// <summary>Danh sách phiếu mượn quá hạn - chưa trả và đã qua DueDate (SCRUM-45).</summary>
    Task<List<Borrow>> GetOverdueAsync();

    /// <summary>Toàn bộ lịch sử mượn của một thành viên (SCRUM-46 - admin xem theo member, SCRUM-47 - member tự xem).</summary>
    Task<List<Borrow>> GetByMemberAsync(string memberId);

    /// <summary>Sách một thành viên đang mượn, chưa trả (SCRUM-48).</summary>
    Task<List<Borrow>> GetActiveByMemberAsync(string memberId);

    /// <summary>Ghi nhận việc mượn sách (SCRUM-44).</summary>
    Task<Borrow> CreateAsync(CreateBorrowModel model);

    /// <summary>Ghi nhận việc trả sách (SCRUM-43).</summary>
    Task<Borrow> ReturnAsync(string borrowId);
}
