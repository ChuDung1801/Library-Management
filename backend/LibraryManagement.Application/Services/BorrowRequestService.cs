using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class BorrowRequestService : IBorrowRequestService
{
    private readonly IBorrowRequestRepository _requestRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBorrowService _borrowService;

    public BorrowRequestService(
        IBorrowRequestRepository requestRepository,
        IBookRepository bookRepository,
        IUserRepository userRepository,
        IBorrowService borrowService)
    {
        _requestRepository = requestRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _borrowService = borrowService;
    }

    public async Task<BorrowRequest> CreateAsync(string memberId, string bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
            throw new NotFoundException("Không tìm thấy sách.");

        if (book.TotalCopies <= 0)
            throw new BusinessRuleException($"Sách '{book.Title}' hiện không còn bản nào khả dụng.");

        var member = await _userRepository.GetByIdAsync(memberId);
        if (member is null || member.Role != Domain.Enums.UserRole.Member)
            throw new BusinessRuleException("Chỉ tài khoản Thành viên mới có thể gửi yêu cầu mượn sách.");

        if (!member.IsActive)
            throw new BusinessRuleException("Tài khoản của bạn đang bị vô hiệu hóa, không thể gửi yêu cầu mượn sách.");

        if (await _requestRepository.HasPendingRequestAsync(memberId, bookId))
            throw new ConflictException("Bạn đã gửi yêu cầu mượn sách này và đang chờ duyệt.");

        var request = new BorrowRequest
        {
            BookId = book.Id,
            BookCode = book.BookCode,
            BookTitle = book.Title,
            MemberId = member.Id,
            MemberFullName = member.FullName,
            MemberPhoneNumber = member.PhoneNumber,
            RequestedAt = DateTime.UtcNow,
            Status = BorrowRequestStatus.Pending
        };

        await _requestRepository.CreateAsync(request);
        return request;
    }

    public async Task<List<BorrowRequest>> GetMineAsync(string memberId) =>
        await _requestRepository.GetByMemberIdAsync(memberId);

    public async Task<List<BorrowRequest>> GetAllAsync() =>
        await _requestRepository.GetAllAsync();

    public async Task<List<BorrowRequest>> GetPendingAsync() =>
        await _requestRepository.GetByStatusAsync(BorrowRequestStatus.Pending);

    public async Task<BorrowRequest> ApproveAsync(string requestId)
    {
        var request = await GetPendingRequestOrThrowAsync(requestId);

        // Tái dùng nguyên logic ghi nhận mượn của Sprint 4 (trừ TotalCopies, tạo Borrow) -
        // đảm bảo hành vi mượn sách chỉ có MỘT nơi xử lý duy nhất, tránh lệch nghiệp vụ.
        var borrow = await _borrowService.CreateAsync(new CreateBorrowModel
        {
            BookId = request.BookId,
            MemberId = request.MemberId
        });

        request.Status = BorrowRequestStatus.Approved;
        request.ProcessedAt = DateTime.UtcNow;
        request.ResultingBorrowId = borrow.Id;
        await _requestRepository.UpdateAsync(request);

        return request;
    }

    public async Task<BorrowRequest> RejectAsync(string requestId, string? note)
    {
        var request = await GetPendingRequestOrThrowAsync(requestId);

        request.Status = BorrowRequestStatus.Rejected;
        request.ProcessedAt = DateTime.UtcNow;
        request.AdminNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        await _requestRepository.UpdateAsync(request);

        return request;
    }

    private async Task<BorrowRequest> GetPendingRequestOrThrowAsync(string requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request is null)
            throw new NotFoundException("Không tìm thấy yêu cầu mượn sách.");

        if (request.Status != BorrowRequestStatus.Pending)
            throw new BusinessRuleException("Yêu cầu này đã được xử lý trước đó.");

        return request;
    }
}
