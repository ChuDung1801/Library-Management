using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class BorrowService : IBorrowService
{
    private const int LoanPeriodDays = 30; // "mượn tối đa 1 tháng" - SKILL_LM.md mục 19

    private readonly IBorrowRepository _borrowRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;

    public BorrowService(
        IBorrowRepository borrowRepository,
        IBookRepository bookRepository,
        IUserRepository userRepository)
    {
        _borrowRepository = borrowRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
    }

    public async Task<List<Borrow>> GetAllAsync() => await _borrowRepository.GetAllAsync();

    public async Task<Borrow> GetByIdAsync(string id)
    {
        var borrow = await _borrowRepository.GetByIdAsync(id);
        if (borrow is null)
            throw new NotFoundException("Không tìm thấy phiếu mượn.");
        return borrow;
    }

    public async Task<List<Borrow>> GetOverdueAsync()
    {
        var active = await _borrowRepository.GetActiveAsync();
        var now = DateTime.UtcNow;
        return active.Where(b => b.DueDate < now).ToList();
    }

    public async Task<List<Borrow>> GetByMemberAsync(string memberId) =>
        await _borrowRepository.GetByMemberIdAsync(memberId);

    public async Task<List<Borrow>> GetActiveByMemberAsync(string memberId) =>
        await _borrowRepository.GetActiveByMemberIdAsync(memberId);

    public async Task<Borrow> CreateAsync(CreateBorrowModel model)
    {
        if (string.IsNullOrWhiteSpace(model.BookId) || string.IsNullOrWhiteSpace(model.MemberId))
            throw new BusinessRuleException("Vui lòng chọn sách và thành viên mượn sách.");

        var book = await _bookRepository.GetByIdAsync(model.BookId);
        if (book is null)
            throw new NotFoundException("Không tìm thấy sách.");

        if (book.TotalCopies <= 0)
            throw new BusinessRuleException($"Sách '{book.Title}' hiện không còn bản nào để cho mượn.");

        var member = await _userRepository.GetByIdAsync(model.MemberId);
        if (member is null || member.Role != UserRole.Member)
            throw new BusinessRuleException("Chỉ có thể ghi nhận mượn sách cho tài khoản Thành viên.");

        if (!member.IsActive)
            throw new BusinessRuleException("Tài khoản thành viên này đang bị vô hiệu hóa, không thể mượn sách.");

        var now = DateTime.UtcNow;
        var borrow = new Borrow
        {
            BookId = book.Id,
            BookCode = book.BookCode,
            BookTitle = book.Title,
            MemberId = member.Id,
            MemberFullName = member.FullName,
            MemberPhoneNumber = member.PhoneNumber,
            BorrowDate = now,
            DueDate = now.AddDays(LoanPeriodDays),
            Status = BorrowStatus.Borrowed,
            CreatedAt = now
        };

        await _borrowRepository.CreateAsync(borrow);

        // "Số sách hiện có" giảm đi 1 khi cho mượn - xem giải thích trong SPRINT4-CHANGES.md
        // về quyết định không thêm field AvailableCopies riêng.
        book.TotalCopies -= 1;
        book.Status = book.TotalCopies > 0 ? BookStatus.Available : BookStatus.Unavailable;
        book.UpdatedAt = now;
        await _bookRepository.UpdateAsync(book);

        return borrow;
    }

    public async Task<Borrow> ReturnAsync(string borrowId)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId);
        if (borrow is null)
            throw new NotFoundException("Không tìm thấy phiếu mượn.");

        if (borrow.Status == BorrowStatus.Returned)
            throw new BusinessRuleException("Phiếu mượn này đã được ghi nhận trả trước đó.");

        var now = DateTime.UtcNow;
        borrow.Status = BorrowStatus.Returned;
        borrow.ReturnDate = now;
        await _borrowRepository.UpdateAsync(borrow);

        var book = await _bookRepository.GetByIdAsync(borrow.BookId);
        if (book is not null)
        {
            book.TotalCopies += 1;
            book.Status = BookStatus.Available;
            book.UpdatedAt = now;
            await _bookRepository.UpdateAsync(book);
        }
        // Nếu sách đã bị xóa khỏi hệ thống sau khi cho mượn, vẫn cho phép ghi nhận trả
        // (không throw) vì đây là thao tác đóng phiếu mượn, không phụ thuộc sách còn tồn tại.

        return borrow;
    }
}
