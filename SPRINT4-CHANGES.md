# Sprint 4 — Member Management (Admin) + Borrow & Return (SCRUM-24, 42, 43, 44, 45)

Zip này **chỉ chứa file mới/thay đổi của Sprint 4**, giữ nguyên đường dẫn tương đối để
copy đè lên project đã có Sprint 1 + 2 + 3.

## User Story hoàn thành

| Story | Endpoint |
|---|---|
| SCRUM-24 Quản trị viên quản lý tài khoản thành viên | `GET /api/members`, `PUT /api/members/{id}/status` |
| SCRUM-42 Quản trị viên cập nhật thông tin thành viên | `PUT /api/members/{id}` |
| SCRUM-44 Quản trị viên ghi nhận việc mượn sách | `POST /api/borrows` |
| SCRUM-43 Quản trị viên ghi nhận việc trả sách | `PUT /api/borrows/{id}/return` |
| SCRUM-45 Quản trị viên xem danh sách sách quá hạn | `GET /api/borrows/overdue` |

## Quyết định kiến trúc quan trọng — ĐỌC TRƯỚC KHI MERGE

**1. Không thêm field `AvailableCopies` vào `Book`.**
Thay vì tách riêng "tổng số bản" và "số bản đang khả dụng" (sẽ cần migrate dữ liệu Mongo
đã seed ở Sprint 2 vì field mới mặc định = 0, làm sai lệch toàn bộ sách cũ), mình giữ
nguyên `Book.TotalCopies` và coi nó LÀ "số sách hiện có khả dụng":
- Mượn sách → `TotalCopies -= 1`
- Trả sách → `TotalCopies += 1`
- `Book.Status` tự tính lại theo `TotalCopies > 0 ? Available : Unavailable` (logic đã có từ Sprint 2)

Đây là đánh đổi có chủ đích để tránh vỡ dữ liệu cũ. Nếu sau này cần tách riêng "tổng số bản
vật lý sở hữu" và "số bản đang cho mượn" (vd để biết thư viện có bao nhiêu cuốn dù đang hết
hàng), sẽ cần một migration script riêng — ngoài phạm vi Sprint 4.

**2. "Quá hạn" không phải trạng thái lưu trong DB.**
`BorrowStatus` chỉ có `Borrowed`/`Returned`. "Quá hạn" = tính động
(`Status == Borrowed && DueDate < DateTime.UtcNow`) ngay tại `BorrowService.GetOverdueAsync()`
và field `IsOverdue` trong `BorrowResponse`. Tránh phải chạy background job cập nhật trạng thái
theo thời gian thực.

**3. Hạn trả = 30 ngày** (hằng số `LoanPeriodDays` trong `BorrowService`), đúng quy tắc
"mượn tối đa 1 tháng" trong SKILL_LM.md mục 19.

**4. `MembersController` (đã có từ Sprint 1) được mở rộng, không tạo controller mới** —
route `/me` (self-service) giữ `[Authorize]` mức controller; 3 route quản trị mới
(`GET /`, `PUT /{id}`, `PUT /{id}/status`) override bằng `[Authorize(Roles = "Admin,Employee")]`
ở từng action.

## File mới

```
backend/LibraryManagement.Domain/Enums/BorrowStatus.cs
backend/LibraryManagement.Domain/Entities/Borrow.cs
backend/LibraryManagement.Infrastructure/Repositories/Interfaces/IBorrowRepository.cs
backend/LibraryManagement.Infrastructure/Repositories/Implementations/BorrowRepository.cs
backend/LibraryManagement.Application/Interfaces/IBorrowService.cs
backend/LibraryManagement.Application/Services/BorrowService.cs
backend/LibraryManagement.API/DTOs/Member/MemberAdminDtos.cs
backend/LibraryManagement.API/DTOs/Borrow/BorrowDtos.cs
backend/LibraryManagement.API/Controllers/BorrowsController.cs
frontend/library-management-web/pages/members.html
frontend/library-management-web/pages/borrows.html
frontend/library-management-web/pages/overdue-books.html
docs/test-cases/sprint4-borrow-return-members.md
```

## File thay đổi (ghi đè file cũ)

```
backend/LibraryManagement.Infrastructure/Data/LibraryDbContext.cs
  -> thêm collection Borrows

backend/LibraryManagement.Infrastructure/Repositories/Interfaces/IUserRepository.cs
  -> thêm GetByRoleAsync(UserRole role)

backend/LibraryManagement.Infrastructure/Repositories/Implementations/UserRepository.cs
  -> implement GetByRoleAsync

backend/LibraryManagement.Application/Interfaces/IUserService.cs
  -> thêm AdminGetMembersAsync, AdminUpdateMemberAsync, AdminSetMemberActiveAsync

backend/LibraryManagement.Application/Services/UserService.cs
  -> implement 3 method trên, refactor UpdateProfileAsync dùng chung helper ApplyUpdateAsync

backend/LibraryManagement.API/Controllers/MembersController.cs
  -> thêm 3 action quản trị (GetAllMembers, AdminUpdateMember, SetMemberActive),
     giữ nguyên 2 action /me cũ từ Sprint 1 KHÔNG đổi

backend/LibraryManagement.API/Program.cs
  -> đăng ký DI: IBorrowRepository, IBorrowService

frontend/library-management-web/js/members.js
  -> từ stub rỗng -> có MembersAdminApi (getAll/update/setActive)

frontend/library-management-web/js/borrows.js
  -> từ stub rỗng -> có BorrowsApi (getAll/getOverdue/create/returnBook)

frontend/library-management-web/pages/home.html
  -> sidebar thêm link Members/Transactions/Overdue, nút Quick Actions nối sang trang tương ứng,
     "View details" ở thẻ Overdue Items trỏ sang overdue-books.html
```

## Cách merge

1. Giải nén đè lên project hiện có.
2. `MembersController.cs` bị thay thế toàn bộ nội dung — nếu bạn tự sửa thêm gì ở đây sau
   Sprint 1, hãy diff trước khi ghi đè (2 action `/me` gốc vẫn y hệt, chỉ thêm phần dưới).
3. Không có package NuGet mới, không cần `dotnet restore` lại.
4. Không cần migrate MongoDB — collection `Borrows` tự tạo khi có phiếu mượn đầu tiên.

## Test nhanh bằng curl

```bash
TOKEN="..."  # token Admin/Employee

# Xem danh sách thành viên
curl http://localhost:5000/api/members -H "Authorization: Bearer $TOKEN"

# Vô hiệu hóa 1 thành viên
curl -X PUT http://localhost:5000/api/members/MEMBER_ID/status \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"isActive": false}'

# Ghi nhận mượn sách
curl -X POST http://localhost:5000/api/borrows \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"bookId":"BOOK_ID","memberId":"MEMBER_ID"}'

# Ghi nhận trả sách
curl -X PUT http://localhost:5000/api/borrows/BORROW_ID/return -H "Authorization: Bearer $TOKEN"

# Xem sách quá hạn
curl http://localhost:5000/api/borrows/overdue -H "Authorization: Bearer $TOKEN"
```

## Lưu ý

- Vẫn chưa `dotnet build` được trong sandbox này (không có quyền `nuget.org`). Không có
  package NuGet mới ở Sprint 4.
- `pages/borrows.html` khi mở modal "Ghi nhận mượn sách" gọi đồng thời `BooksApi.getAll()`
  và `MembersAdminApi.getAll()` rồi lọc phía client (`totalCopies > 0`, `isActive`) — với
  dữ liệu lớn hơn nên cân nhắc thêm filter phía server ở sprint sau, hiện tại quy mô nhóm 3
  người + dữ liệu demo nên chưa cần tối ưu.
