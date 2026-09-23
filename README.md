# Scholaris Library — Library Management System

Hệ thống quản lý thư viện (.NET 10 + MongoDB + HTML/CSS/JS thuần).
Cấu trúc thư mục theo `FolderContruct.md`, nghiệp vụ theo `SKILL_LM.md`.

## Trạng thái: Sprint 1 hoàn thành — Epic 01: Account Management

| User Story | Trạng thái |
|---|---|
| SCRUM-16 Thành viên tạo tài khoản | ✅ `POST /api/auth/register` |
| SCRUM-29 Thành viên đăng nhập | ✅ `POST /api/auth/login` |
| SCRUM-30 Thành viên cập nhật thông tin cá nhân | ✅ `GET/PUT /api/members/me` |
| SCRUM-31 Khách tạo tài khoản | ✅ dùng chung endpoint register (tạo role Member) |

Sprint 2 trở đi (Book Management, Category, Member list quản trị, Borrow/Return, History...)
sẽ được triển khai khi bạn yêu cầu tiếp — **chưa động vào phạm vi này** theo đúng nguyên tắc
"bám sát Product Backlog, tránh Scope Creep" trong `SKILL_LM.md`.

---

## 1. Yêu cầu môi trường

- .NET 10 SDK
- MongoDB chạy tại `mongodb://localhost:27017` (mặc định, có thể đổi trong `appsettings.json`)

## 2. Chạy Backend

```bash
cd backend/LibraryManagement.API
dotnet restore   # cần internet để tải NuGet packages (MongoDB.Driver, JwtBearer, Swashbuckle...)
dotnet run
```

API chạy tại `http://localhost:5000`, Swagger UI tại `http://localhost:5000/swagger`.

> **Lưu ý:** dự án được viết trong môi trường sandbox không có quyền truy cập `nuget.org`
> nên chưa build/verify được. Nếu `dotnet restore` báo lỗi version package không tồn tại
> (MongoDB.Driver, Microsoft.IdentityModel.Tokens, Swashbuckle.AspNetCore...), hãy đổi sang
> version mới nhất hiện có bằng `dotnet add package <TênGói>`.

Khi chạy lần đầu, `SeedDataService` sẽ tự động nạp 6 tài khoản mẫu từ `users.json` vào MongoDB
(chỉ chạy nếu collection `Users` đang rỗng):

| Tài khoản đăng nhập | Mật khẩu | Vai trò |
|---|---|---|
| admin | admin123 | Admin |
| employee01 / employee02 | employee123 | Employee |
| minhanh.pham / baohg / hadothu | user123 | Member |

## 3. Chạy Frontend

Frontend là HTML/CSS/JS thuần, không cần build. Mở bằng một static server bất kỳ (Live Server,
`python -m http.server`, ...) tại thư mục `frontend/library-management-web`, ví dụ:

```bash
cd frontend/library-management-web
python3 -m http.server 5500
```

Sau đó mở `http://localhost:5500/pages/login.html`.

- `pages/login.html` — đăng nhập (SCRUM-29)
- `pages/register.html` — tạo tài khoản (SCRUM-16 / SCRUM-31)
- `pages/profile.html` — xem & cập nhật thông tin cá nhân (SCRUM-30)
- `pages/home.html` — dashboard quản trị (Admin/Employee), giữ nguyên UI demo gốc, Sprint 2+ sẽ nối dữ liệu thật

`js/api.js` mặc định gọi API tại `http://localhost:5000/api` — đổi `API_BASE_URL` nếu backend
chạy ở cổng khác.

## 4. Test nhanh bằng curl

```bash
# Đăng ký
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Nguyen Van A","email":"a@example.com","password":"123456","phoneNumber":"0912345678"}'

# Đăng nhập
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"admin","password":"admin123"}'

# Xem hồ sơ (thay TOKEN bằng token trả về ở bước login)
curl http://localhost:5000/api/members/me -H "Authorization: Bearer TOKEN"
```

## 5. Test Case đã chuẩn bị (xem chi tiết trong `docs/test-cases/`)

- **Positive:** đăng ký/đăng nhập/cập nhật với dữ liệu hợp lệ → thành công.
- **Negative:** thiếu trường bắt buộc, email sai định dạng, trùng email/username, sai mật khẩu.
- **Boundary:** tên 50 ký tự (hợp lệ) / 51 ký tự (từ chối), mật khẩu 6 ký tự (hợp lệ) / 5 ký tự (từ chối).
- **Permission:** gọi `GET/PUT /api/members/me` không kèm token → 401 Unauthorized.
