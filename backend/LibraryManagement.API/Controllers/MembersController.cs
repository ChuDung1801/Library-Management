using System.Security.Claims;
using LibraryManagement.API.DTOs.Auth;
using LibraryManagement.API.DTOs.Borrow;
using LibraryManagement.API.DTOs.Member;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/members")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBorrowService _borrowService;

    public MembersController(IUserService userService, IBorrowService borrowService)
    {
        _userService = userService;
        _borrowService = borrowService;
    }

    /// <summary>Lấy thông tin cá nhân của người dùng đang đăng nhập.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.GetByIdAsync(userId);
        return Ok(Map(user));
    }

    /// <summary>Cập nhật thông tin cá nhân (SCRUM-30).</summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userService.UpdateProfileAsync(userId, new UpdateProfileModel
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        });
        return Ok(Map(user));
    }

    private static UserResponse Map(Domain.Entities.User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Username = user.Username,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Role = user.Role.ToString()
    };

    // ---------------------------------------------------------------------
    // Các endpoint dưới đây dành riêng cho Admin/Employee quản lý thành viên
    // (SCRUM-24, SCRUM-42) - override [Authorize] mức controller bằng
    // [Authorize(Roles = "Admin,Employee")] chặt hơn cho từng action.
    // ---------------------------------------------------------------------

    /// <summary>Xem danh sách tài khoản thành viên (SCRUM-24).</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<List<MemberResponse>>> GetAllMembers()
    {
        var members = await _userService.AdminGetMembersAsync();
        return Ok(members.Select(MapMember).ToList());
    }

    /// <summary>Admin cập nhật thông tin một thành viên (SCRUM-42).</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<MemberResponse>> AdminUpdateMember(string id, [FromBody] UpdateProfileRequest request)
    {
        var member = await _userService.AdminUpdateMemberAsync(id, new UpdateProfileModel
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        });
        return Ok(MapMember(member));
    }

    /// <summary>
    /// Kích hoạt/vô hiệu hóa tài khoản thành viên - một phần của "quản lý tài khoản
    /// thành viên" (SCRUM-24). Tài khoản bị vô hiệu hóa không đăng nhập/mượn sách được.
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<MemberResponse>> SetMemberActive(string id, [FromBody] SetMemberActiveRequest request)
    {
        var member = await _userService.AdminSetMemberActiveAsync(id, request.IsActive);
        return Ok(MapMember(member));
    }

    private static MemberResponse MapMember(Domain.Entities.User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Username = user.Username,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };

    // ---------------------------------------------------------------------
    // Thành viên tự xem lịch sử mượn sách của mình (SCRUM-47, 48).
    // Không giới hạn role riêng vì đây là dữ liệu tự-scope (chỉ trả về của chính
    // người gọi API, lấy từ claim NameIdentifier) - vô hại nếu Admin/Employee cũng xem được.
    // ---------------------------------------------------------------------

    /// <summary>Lịch sử mượn sách của bản thân (SCRUM-47).</summary>
    [HttpGet("me/borrows")]
    public async Task<ActionResult<List<BorrowResponse>>> GetMyBorrows()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var borrows = await _borrowService.GetByMemberAsync(userId);
        return Ok(borrows.Select(MapBorrow).ToList());
    }

    /// <summary>Sách bản thân đang mượn, chưa trả (SCRUM-48).</summary>
    [HttpGet("me/borrows/active")]
    public async Task<ActionResult<List<BorrowResponse>>> GetMyActiveBorrows()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var borrows = await _borrowService.GetActiveByMemberAsync(userId);
        return Ok(borrows.Select(MapBorrow).ToList());
    }

    private static BorrowResponse MapBorrow(Domain.Entities.Borrow b) => new()
    {
        Id = b.Id,
        BookId = b.BookId,
        BookCode = b.BookCode,
        BookTitle = b.BookTitle,
        MemberId = b.MemberId,
        MemberFullName = b.MemberFullName,
        MemberPhoneNumber = b.MemberPhoneNumber,
        BorrowDate = b.BorrowDate,
        DueDate = b.DueDate,
        ReturnDate = b.ReturnDate,
        Status = b.Status.ToString(),
        IsOverdue = b.ReturnDate is null && b.DueDate < DateTime.UtcNow
    };
}
