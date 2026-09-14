# Library Management System

Cấu trúc dự án được tổ chức theo `FolderContruct.md`.

- Frontend: HTML/CSS/JavaScript cơ bản tại `frontend/library-management-web/`.
- Backend: ASP.NET Core Web API theo các tầng API, Application, Domain và Infrastructure.
- Tests: Unit và Integration.
- Docs: tài liệu thiết kế, API và test case.

## Quy trình chạy

Yêu cầu: cài .NET 10 SDK và Python.

### MongoDB và dữ liệu mẫu

Backend sử dụng MongoDB mặc định tại `mongodb://localhost:27017`, với database `LibraryManagement`.
Khi API khởi động, `Data/SeedData.cs` tự động upsert dữ liệu mẫu vào các collection `users`, `members`, `books`, `categories`, `borrows` và `borrowDetails`.
Seed có thể chạy lại an toàn mà không tạo bản ghi trùng. Có thể đổi connection string hoặc tên database trong `backend/LibraryManagement.API/appsettings.json`.

Schema chính:

- `users`: họ tên, username, mật khẩu hash, số điện thoại, email, role và trạng thái.
- `members`: hồ sơ thành viên liên kết với `users` qua `userId`.
- `books`: mã sách, tên, tác giả, nhà xuất bản, năm xuất bản, mô tả, tóm tắt, thể loại và số bản.
- `categories`: thể loại sách.
- `borrows`/`borrowDetails`: phiếu mượn, sách mượn, ngày mượn, hạn trả và ngày trả.
- `extensionRequests`: yêu cầu gia hạn gắn với phiếu mượn.

Sau khi MongoDB đang chạy, kiểm tra dữ liệu bằng các endpoint:

```text
GET /api/books
GET /api/books?search=code
GET /api/members
POST /api/books
PUT /api/books/{id}
DELETE /api/books/{id}
PUT /api/members/{id}
DELETE /api/members/{id}
POST /api/auth/register
POST /api/auth/login
```

Các request ghi dữ liệu phải gửi JSON. Ví dụ đăng ký:

```json
{
	"fullName": "Le Thi Hoa",
	"username": "lethihoa",
	"password": "mat-khau-cua-ban",
	"phoneNumber": "0933333333",
	"email": "lethihoa@library.local"
}
```

Mật khẩu được hash bằng PBKDF2 trước khi lưu vào MongoDB, không lưu plaintext.

### Chạy Backend

```powershell
cd backend/LibraryManagement.API
dotnet restore
dotnet run
```

API mặc định chạy tại URL được hiển thị trong terminal.

### Chạy Frontend

Mở terminal thứ hai:

```powershell
cd frontend/library-management-web
python -m http.server 5500
```

Mở trình duyệt tại `http://localhost:5500`.
