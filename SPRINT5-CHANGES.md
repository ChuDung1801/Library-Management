# Sprint 5 — History Tracking & Book Borrowing Requests (SCRUM-46..50)

Zip này **chỉ chứa file mới/thay đổi của Sprint 5**, giữ nguyên đường dẫn tương đối để
copy đè lên project đã có Sprint 1 + 2 + 3 + 4.

## User Story

| Story | Trạng thái |
|---|---|
| SCRUM-46 Admin xem lịch sử mượn sách | Mới: trang `borrow-history.html` + filter `?memberId=` trên `GET /api/borrows` |
| SCRUM-47 Thành viên xem lịch sử mượn sách của mình | Mới: `GET /api/members/me/borrows` |
| SCRUM-48 Thành viên xem sách đang mượn | Mới: `GET /api/members/me/borrows/active` |
| SCRUM-49 Thành viên gửi yêu cầu mượn sách | Mới: entity `BorrowRequest` + `POST /api/borrow-requests` + Admin duyệt/từ chối |
| SCRUM-50 Khách xem chi tiết sách | **Đã xong từ Sprint 3** (`GET /api/catalog/books/{id}` public) - không có code mới, chỉ thêm regression test case |

## Quyết định kiến trúc quan trọng — ĐỌC TRƯỚC KHI MERGE

**SCRUM-49 là luồng "yêu cầu → duyệt", không phải Member tự mượn thẳng.**
Theo SKILL_LM.md mục 3.1/10, "ghi nhận việc mượn sách" vẫn là đặc quyền Admin/Employee.
Vì vậy:
1. Member gọi `POST /api/borrow-requests` → tạo `BorrowRequest` với status `Pending`.
2. Admin/Employee xem hàng đợi (`GET /api/borrow-requests/pending`), rồi:
   - **Duyệt** (`PUT /api/borrow-requests/{id}/approve`) → `BorrowRequestService.ApproveAsync`
     gọi thẳng `IBorrowService.CreateAsync` (logic y hệt Sprint 4, KHÔNG viết lại) để tạo
     `Borrow` thật + trừ `Book.TotalCopies`.
   - **Từ chối** (`PUT /api/borrow-requests/{id}/reject`) → chỉ đổi status, không đụng vào kho sách.

Điều này giữ đúng nguyên tắc "một hành vi nghiệp vụ chỉ xử lý ở một nơi duy nhất" — nếu sau
này cần đổi quy tắc mượn sách (vd thời hạn, điều kiện), chỉ sửa `BorrowService`, không phải
sửa 2 chỗ.

## File mới

```
backend/LibraryManagement.Domain/Enums/BorrowRequestStatus.cs
backend/LibraryManagement.Domain/Entities/BorrowRequest.cs
backend/LibraryManagement.Infrastructure/Repositories/Interfaces/IBorrowRequestRepository.cs
backend/LibraryManagement.Infrastructure/Repositories/Implementations/BorrowRequestRepository.cs
backend/LibraryManagement.Application/Interfaces/IBorrowRequestService.cs
backend/LibraryManagement.Application/Services/BorrowRequestService.cs
backend/LibraryManagement.API/DTOs/Borrow/BorrowRequestDtos.cs
backend/LibraryManagement.API/Controllers/BorrowRequestsController.cs
frontend/library-management-web/js/borrow-requests.js
frontend/library-management-web/pages/my-borrows.html
frontend/library-management-web/pages/borrow-history.html
frontend/library-management-web/pages/borrow-requests.html
docs/test-cases/sprint5-history-borrow-requests.md
```

## File thay đổi (ghi đè file cũ)

```
backend/LibraryManagement.Infrastructure/Data/LibraryDbContext.cs
  -> thêm collection BorrowRequests

backend/LibraryManagement.Infrastructure/Repositories/Interfaces/IBorrowRepository.cs
  -> thêm GetByMemberIdAsync, GetActiveByMemberIdAsync

backend/LibraryManagement.Infrastructure/Repositories/Implementations/BorrowRepository.cs
  -> implement 2 method trên

backend/LibraryManagement.Application/Interfaces/IBorrowService.cs
  -> thêm GetByMemberAsync, GetActiveByMemberAsync

backend/LibraryManagement.Application/Services/BorrowService.cs
  -> implement 2 method trên (chỉ thêm, KHÔNG đổi CreateAsync/ReturnAsync đã có từ Sprint 4)

backend/LibraryManagement.API/Controllers/MembersController.cs
  -> thêm GET me/borrows (SCRUM-47), GET me/borrows/active (SCRUM-48);
     constructor giờ nhận thêm IBorrowService - nếu bạn tự thêm constructor param khác
     ở Sprint 1-4, hãy merge tay đoạn này

backend/LibraryManagement.API/Controllers/BorrowsController.cs
  -> GetAll() nhận thêm [FromQuery] string? memberId (SCRUM-46)

backend/LibraryManagement.API/Program.cs
  -> đăng ký DI: IBorrowRequestRepository, IBorrowRequestService

frontend/library-management-web/js/borrows.js
  -> thêm BorrowsApi.getMine() / getMineActive()

frontend/library-management-web/pages/book-detail.html
  -> thêm nút "Gửi yêu cầu mượn sách" (chỉ hiện với Member đã đăng nhập, ẩn với
     Admin/Employee, đổi thành "Đăng nhập để mượn sách" với Guest)

frontend/library-management-web/pages/catalog.html
  -> header nav thêm link "My Borrows" khi đăng nhập bằng role Member

frontend/library-management-web/pages/home.html, books.html, categories.html,
frontend/library-management-web/pages/members.html, borrows.html, overdue-books.html
  -> sidebar đồng bộ: books.html/categories.html được nối link Members/Transactions thật
     (trước đây là <div> tĩnh không click được - lỗi sót từ Sprint 2/3); toàn bộ 6 trang
     admin đều được thêm 2 link mới "Requests" và "History"
```

## Cách merge

1. Giải nén đè lên project hiện có.
2. **Lưu ý `MembersController.cs`**: constructor đổi chữ ký (`IUserService` → thêm
   `IBorrowService`). Nếu bạn tự sửa file này sau Sprint 1/4, merge tay phần constructor.
3. Không có package NuGet mới, không cần `dotnet restore` lại.
4. Không cần migrate MongoDB — collection `BorrowRequests` tự tạo khi có yêu cầu đầu tiên.

## Test nhanh bằng curl

```bash
MEMBER_TOKEN="..."   # token của tài khoản role Member
ADMIN_TOKEN="..."    # token Admin/Employee

# Member gửi yêu cầu mượn sách
curl -X POST http://localhost:5000/api/borrow-requests \
  -H "Authorization: Bearer $MEMBER_TOKEN" -H "Content-Type: application/json" \
  -d '{"bookId":"BOOK_ID"}'

# Member xem lịch sử / đang mượn / yêu cầu của mình
curl http://localhost:5000/api/members/me/borrows -H "Authorization: Bearer $MEMBER_TOKEN"
curl http://localhost:5000/api/members/me/borrows/active -H "Authorization: Bearer $MEMBER_TOKEN"
curl http://localhost:5000/api/borrow-requests/mine -H "Authorization: Bearer $MEMBER_TOKEN"

# Admin xem hàng đợi & duyệt
curl http://localhost:5000/api/borrow-requests/pending -H "Authorization: Bearer $ADMIN_TOKEN"
curl -X PUT http://localhost:5000/api/borrow-requests/REQUEST_ID/approve -H "Authorization: Bearer $ADMIN_TOKEN"

# Admin xem lịch sử theo thành viên
curl "http://localhost:5000/api/borrows?memberId=MEMBER_ID" -H "Authorization: Bearer $ADMIN_TOKEN"
```

Mở `frontend/library-management-web/pages/book-detail.html?id=BOOK_ID` khi đăng nhập bằng
tài khoản Member (vd `minhanh.pham` / `user123` từ seed data Sprint 1) để thấy nút gửi yêu
cầu mượn; vào `pages/my-borrows.html` để xem trạng thái; đăng nhập Admin vào
`pages/borrow-requests.html` để duyệt.

## Lưu ý

- Vẫn chưa `dotnet build` được trong sandbox này (không có quyền `nuget.org`). Không có
  package NuGet mới ở Sprint 5.
- `BorrowRequestService.ApproveAsync` gọi lại `BorrowService.CreateAsync`, nghĩa là mọi
  validate của Sprint 4 (sách còn hàng, member active, member đúng role) được áp dụng lại
  tại thời điểm DUYỆT — nếu giữa lúc Member gửi yêu cầu và lúc Admin duyệt mà sách hết hàng,
  `ApproveAsync` sẽ ném lỗi 400 thay vì âm thầm tạo Borrow sai.
