using LibraryManagement.API.DTOs.Borrow;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Nghiệp vụ mượn/trả sách - Epic 04 (SCRUM-43, 44, 45). Toàn bộ endpoint chỉ dành cho
/// Admin/Employee, vì đây là thao tác ghi nhận tại quầy thủ thư, không phải Member tự thao tác
/// (Member tự xem sách đang mượn/lịch sử thuộc Sprint 5).
/// </summary>
[ApiController]
[Route("api/borrows")]
[Authorize(Roles = "Admin,Employee")]
public class BorrowsController : ControllerBase
{
    private readonly IBorrowService _borrowService;

    public BorrowsController(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    /// <summary>
    /// Xem lịch sử mượn sách (SCRUM-46). Truyền "memberId" để lọc lịch sử của một
    /// thành viên cụ thể, hoặc bỏ trống để xem toàn bộ.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BorrowResponse>>> GetAll([FromQuery] string? memberId)
    {
        var borrows = string.IsNullOrWhiteSpace(memberId)
            ? await _borrowService.GetAllAsync()
            : await _borrowService.GetByMemberAsync(memberId);
        return Ok(borrows.Select(Map).ToList());
    }

    /// <summary>Danh sách sách quá hạn (SCRUM-45).</summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<List<BorrowResponse>>> GetOverdue()
    {
        var overdue = await _borrowService.GetOverdueAsync();
        return Ok(overdue.Select(Map).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BorrowResponse>> GetById(string id)
    {
        var borrow = await _borrowService.GetByIdAsync(id);
        return Ok(Map(borrow));
    }

    /// <summary>Ghi nhận việc mượn sách (SCRUM-44).</summary>
    [HttpPost]
    public async Task<ActionResult<BorrowResponse>> Create([FromBody] CreateBorrowRequest request)
    {
        var borrow = await _borrowService.CreateAsync(new CreateBorrowModel
        {
            BookId = request.BookId,
            MemberId = request.MemberId
        });
        return CreatedAtAction(nameof(GetById), new { id = borrow.Id }, Map(borrow));
    }

    /// <summary>Ghi nhận việc trả sách (SCRUM-43).</summary>
    [HttpPut("{id}/return")]
    public async Task<ActionResult<BorrowResponse>> Return(string id)
    {
        var borrow = await _borrowService.ReturnAsync(id);
        return Ok(Map(borrow));
    }

    private static BorrowResponse Map(Borrow borrow) => new()
    {
        Id = borrow.Id,
        BookId = borrow.BookId,
        BookCode = borrow.BookCode,
        BookTitle = borrow.BookTitle,
        MemberId = borrow.MemberId,
        MemberFullName = borrow.MemberFullName,
        MemberPhoneNumber = borrow.MemberPhoneNumber,
        BorrowDate = borrow.BorrowDate,
        DueDate = borrow.DueDate,
        ReturnDate = borrow.ReturnDate,
        Status = borrow.Status.ToString(),
        IsOverdue = borrow.ReturnDate is null && borrow.DueDate < DateTime.UtcNow
    };
}
