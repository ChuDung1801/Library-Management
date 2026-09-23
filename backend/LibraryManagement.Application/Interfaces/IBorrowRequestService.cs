using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Interfaces;

public interface IBorrowRequestService
{
    /// <summary>Thành viên gửi yêu cầu mượn sách (SCRUM-49).</summary>
    Task<BorrowRequest> CreateAsync(string memberId, string bookId);

    /// <summary>Thành viên xem các yêu cầu mượn sách của chính mình (kèm trạng thái).</summary>
    Task<List<BorrowRequest>> GetMineAsync(string memberId);

    Task<List<BorrowRequest>> GetAllAsync();

    /// <summary>Danh sách yêu cầu đang chờ duyệt - hàng đợi cho Admin.</summary>
    Task<List<BorrowRequest>> GetPendingAsync();

    /// <summary>Admin duyệt yêu cầu - tạo Borrow thật (tái dùng BorrowService.CreateAsync).</summary>
    Task<BorrowRequest> ApproveAsync(string requestId);

    /// <summary>Admin từ chối yêu cầu, có thể kèm lý do.</summary>
    Task<BorrowRequest> RejectAsync(string requestId, string? note);
}
