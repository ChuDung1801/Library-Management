# Scholaris Library — Hệ thống quản lý thư viện

## Giới thiệu

Scholaris Library là hệ thống quản lý thư viện giúp quản trị viên quản lý sách, thể loại,
thành viên và toàn bộ nghiệp vụ mượn/trả sách; đồng thời cho phép thành viên và khách truy
cập tìm kiếm, xem thông tin sách và theo dõi việc mượn sách của mình.

**Product Goal:** Xây dựng hệ thống quản lý thư viện giúp quản lý sách, thành viên và hoạt
động mượn/trả một cách hiệu quả, đồng thời hỗ trợ thành viên và khách hàng dễ dàng tìm kiếm
và theo dõi thông tin sách.

## Đối tượng sử dụng

| Vai trò | Quyền hạn chính |
|---|---|
| **Admin** | Toàn quyền: quản lý sách, thể loại, thành viên, ghi nhận mượn/trả, duyệt yêu cầu mượn, xem lịch sử & sách quá hạn |
| **Employee** (nhân viên) | Giống Admin ở các nghiệp vụ vận hành hằng ngày (sách, mượn/trả, thành viên) |
| **Member** (thành viên) | Tìm kiếm/xem sách, tự cập nhật hồ sơ, xem sách đang mượn & lịch sử mượn của mình, gửi yêu cầu mượn sách |
| **Guest** (khách) | Tìm kiếm/xem sách công khai, đăng ký tài khoản để trở thành Member |

## Kiến trúc hệ thống

```
Frontend (HTML/CSS/JS thuần)
        ↓ REST API (JWT Bearer)
API (Controllers, DTOs)
        ↓
Application (Services, business rules, validation)
        ↓
Infrastructure (Repositories, MongoDB)
        ↓
Domain (Entities, Enums, Exceptions - không phụ thuộc tầng nào khác)
```

Kiến trúc nhiều tầng (Clean Architecture rút gọn), tách biệt rõ Frontend – Backend –
Database, ưu tiên đơn giản, dễ bảo trì cho nhóm phát triển nhỏ.

## Công nghệ sử dụng

- **Backend:** .NET 10 Web API, MongoDB.Driver, JWT Bearer Authentication
- **Database:** MongoDB
- **Frontend:** HTML/CSS/JavaScript thuần (không dùng framework), gọi API qua `fetch`
- **Mật khẩu:** băm bằng PBKDF2 (built-in .NET, không phụ thuộc thư viện ngoài)

## Chức năng theo Epic

### Epic 01 — Account Management
Đăng ký tài khoản (Member/Guest), đăng nhập, cập nhật thông tin cá nhân.

### Epic 02 — Book Management
Thêm/sửa/xóa/xem danh sách sách, thêm thể loại sách (chỉ Admin/Employee).

### Epic — Search & View Books
Tìm kiếm sách (Admin, Member), xem chi tiết & tình trạng sách (Member),
xem danh sách sách công khai không cần đăng nhập (Guest).

### Epic 04 — Borrow & Return
Ghi nhận mượn sách, ghi nhận trả sách, xem danh sách sách quá hạn (Admin/Employee).
Quy tắc: một sách mượn tối đa 1 tháng.

### Epic — Member Management (Admin)
Xem danh sách tài khoản thành viên, cập nhật thông tin thành viên, kích hoạt/vô hiệu hóa
tài khoản.

### Epic 05 — History Tracking & Book Borrowing Requests
Admin xem lịch sử mượn sách (toàn bộ hoặc theo từng thành viên); Member xem lịch sử và
sách đang mượn của chính mình; Member gửi yêu cầu mượn sách để Admin duyệt/từ chối.

## Cấu trúc dữ liệu chính

- **User** — tài khoản (Admin/Employee/Member), có trạng thái kích hoạt riêng
- **Book** — sách (mã sách, tên, tác giả, nhà xuất bản, năm XB, số lượng, trạng thái)
- **Category** — thể loại sách
- **Borrow** — phiếu mượn (ngày mượn, hạn trả, ngày trả thực tế, trạng thái)
- **BorrowRequest** — yêu cầu mượn sách do Member gửi, chờ Admin duyệt

## Nguyên tắc thiết kế

- Bám sát Product Backlog, không tự ý mở rộng phạm vi (tránh Scope Creep)
- Ưu tiên Must Have trước Should/Could Have
- Kiến trúc và cơ sở dữ liệu vừa đủ, không phức tạp hóa
- Mọi thao tác nguy hiểm (xóa, vô hiệu hóa tài khoản) đều có xác nhận trước khi thực hiện
- Một hành vi nghiệp vụ chỉ xử lý ở một nơi duy nhất (ví dụ: duyệt yêu cầu mượn sách tái sử
  dụng đúng logic ghi nhận mượn sách, không viết lại)
