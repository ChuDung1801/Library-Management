using System.Security.Claims;
using LibraryManagement.API.DTOs.Borrow;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Yêu cầu mượn sách do Thành viên gửi (SCRUM-49). Member CHỈ tạo/xem yêu cầu của chính
/// mình; duyệt/từ chối vẫn là đặc quyền Admin/Employee (giữ đúng phân quyền SKILL_LM.md).
/// </summary>
[ApiController]
[Route("api/borrow-requests")]
[Authorize]
public class BorrowRequestsController : ControllerBase
{
    private readonly IBorrowRequestService _requestService;

    public BorrowRequestsController(IBorrowRequestService requestService)
    {
        _requestService = requestService;
    }

    /// <summary>Thành viên gửi yêu cầu mượn sách (SCRUM-49).</summary>
    [HttpPost]
    [Authorize(Roles = "Member")]
    public async Task<ActionResult<BorrowRequestResponse>> Create([FromBody] CreateBorrowRequestRequest request)
    {
        var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var created = await _requestService.CreateAsync(memberId, request.BookId);
        return Ok(Map(created));
    }

    /// <summary>Thành viên xem các yêu cầu mượn sách của chính mình + trạng thái duyệt.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = "Member")]
    public async Task<ActionResult<List<BorrowRequestResponse>>> GetMine()
    {
        var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var requests = await _requestService.GetMineAsync(memberId);
        return Ok(requests.Select(Map).ToList());
    }

    /// <summary>Admin xem toàn bộ yêu cầu mượn sách.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<List<BorrowRequestResponse>>> GetAll()
    {
        var requests = await _requestService.GetAllAsync();
        return Ok(requests.Select(Map).ToList());
    }

    /// <summary>Admin xem hàng đợi yêu cầu đang chờ duyệt.</summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<List<BorrowRequestResponse>>> GetPending()
    {
        var requests = await _requestService.GetPendingAsync();
        return Ok(requests.Select(Map).ToList());
    }

    /// <summary>Admin duyệt yêu cầu - tạo Borrow thật, trừ kho sách.</summary>
    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<BorrowRequestResponse>> Approve(string id)
    {
        var updated = await _requestService.ApproveAsync(id);
        return Ok(Map(updated));
    }

    /// <summary>Admin từ chối yêu cầu.</summary>
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<BorrowRequestResponse>> Reject(string id, [FromBody] RejectBorrowRequestRequest request)
    {
        var updated = await _requestService.RejectAsync(id, request.Note);
        return Ok(Map(updated));
    }

    private static BorrowRequestResponse Map(BorrowRequest r) => new()
    {
        Id = r.Id,
        BookId = r.BookId,
        BookCode = r.BookCode,
        BookTitle = r.BookTitle,
        MemberId = r.MemberId,
        MemberFullName = r.MemberFullName,
        MemberPhoneNumber = r.MemberPhoneNumber,
        RequestedAt = r.RequestedAt,
        Status = r.Status.ToString(),
        ProcessedAt = r.ProcessedAt,
        AdminNote = r.AdminNote,
        ResultingBorrowId = r.ResultingBorrowId
    };
}
