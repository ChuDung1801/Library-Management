# Chạy Scholaris Library Management System

## 1. MongoDB

Đảm bảo MongoDB đang chạy tại `mongodb://localhost:27017` (nếu đã cài & chạy sẵn thì bỏ qua bước này).

## 2. Backend (.NET API)

Mở terminal 1:

```bash
cd LibraryManagement/backend/LibraryManagement.API
dotnet restore
dotnet run
```

- API chạy tại: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- Lần chạy đầu tiên sẽ tự động seed dữ liệu mẫu (users.json, books.json) vào MongoDB nếu database đang rỗng.

**Nếu gặp lỗi `Cannot consume scoped service ... from singleton ...`:** đã fix trong `SeedDataService.cs` (dùng `IServiceScopeFactory` thay vì inject trực tiếp Scoped service vào Singleton `IHostedService`). Đảm bảo bạn đang dùng bản mới nhất của file này.

## 3. Frontend (HTML/CSS/JS thuần)

Không mở trực tiếp file `.html` (double-click) vì sẽ bị lỗi CORS do origin `file://`. Phải chạy qua một static server:

**Cách A — VS Code Live Server:**

1. Mở thư mục `LibraryManagement/frontend/library-management-web` trong VS Code.
2. Chuột phải `pages/login.html` → **Open with Live Server**.
3. Truy cập theo địa chỉ Live Server hiện ra (thường `http://127.0.0.1:5500/pages/login.html`).

**Cách B — Python (nếu có, dùng `py` thay vì `python` trên Windows):**

```bash
cd LibraryManagement/frontend/library-management-web
py -m http.server 5500
```

**Cách C — Node.js:**

```bash
cd LibraryManagement/frontend/library-management-web
npx serve -l 5500 .
```

Nếu frontend không gọi được API, kiểm tra `js/api.js`, biến `API_BASE_URL` đang trỏ đúng `http://localhost:5000/api` hay chưa (đổi sang `127.0.0.1` nếu Live Server chạy bằng host đó).

## 4. Truy cập & tài khoản test

| Trang                           | URL                   |
| ------------------------------- | --------------------- |
| Đăng nhập                       | `pages/login.html`    |
| Đăng ký                         | `pages/register.html` |
| Danh mục sách (Guest xem được)  | `pages/catalog.html`  |
| Trang quản trị (Admin/Employee) | `pages/home.html`     |

| Tài khoản    | Mật khẩu    | Vai trò  |
| ------------ | ----------- | -------- |
| admin        | admin123    | Admin    |
| employee01   | employee123 | Employee |
| minhanh.pham | user123     | Member   |

## 5. Thứ tự kiểm thử gợi ý

1. Đăng nhập Admin → vào `books.html` thêm/sửa/xóa sách, `categories.html` thêm thể loại.
2. Đăng nhập Member (`minhanh.pham`) → vào `catalog.html` tìm sách, xem chi tiết, gửi yêu cầu mượn.
3. Quay lại Admin → `borrow-requests.html` duyệt yêu cầu, `borrows.html` ghi nhận trả sách.
4. Kiểm tra `overdue-books.html` và `my-borrows.html` (phía Member) để xác nhận dữ liệu đồng bộ.
