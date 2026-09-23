# Sprint 3 — Search & View Books (SCRUM-37..41)

Zip này **chỉ chứa file mới/thay đổi của Sprint 3**, giữ nguyên đường dẫn tương đối để
copy đè lên project đã có Sprint 1 + Sprint 2.

## User Story hoàn thành

| Story | Ai dùng | Endpoint |
|---|---|---|
| SCRUM-37 Quản trị viên tìm kiếm sách | Admin/Employee | `GET /api/books?keyword=` |
| SCRUM-38 Thành viên tìm kiếm sách | Member | `GET /api/catalog/books?keyword=` |
| SCRUM-39 Thành viên xem chi tiết sách | Member | `GET /api/catalog/books/{id}` |
| SCRUM-40 Thành viên xem tình trạng sách | Member | field `status` trong response chi tiết |
| SCRUM-41 Khách xem danh sách sách | Guest (không cần đăng nhập) | `GET /api/catalog/books` |

**Quyết định kiến trúc:** `CatalogController` mới, hoàn toàn public (`[AllowAnonymous]`,
không `[Authorize]`), tách biệt khỏi `BooksController` quản trị (vẫn giữ nguyên
`[Authorize(Roles = "Admin,Employee")]` từ Sprint 2) — đảm bảo Guest/Member chỉ đọc được,
không có bất kỳ endpoint ghi (POST/PUT/DELETE) nào lộ ra ngoài.

`Program.cs` **không cần sửa** — controller mới tự được `MapControllers()` nhận diện,
không có global authorize filter nên controller không gắn `[Authorize]` mặc định là public.

## File mới

```
backend/LibraryManagement.API/DTOs/Book/CatalogBookResponse.cs
backend/LibraryManagement.API/Controllers/CatalogController.cs
frontend/library-management-web/js/catalog.js
docs/test-cases/sprint3-search-view-books.md
```

## File thay đổi (ghi đè file cũ)

```
backend/LibraryManagement.Infrastructure/Repositories/Interfaces/IBookRepository.cs
  -> thêm SearchAsync(string keyword)

backend/LibraryManagement.Infrastructure/Repositories/Implementations/BookRepository.cs
  -> implement SearchAsync (regex không phân biệt hoa/thường trên Title/Author/BookCode)

backend/LibraryManagement.Application/Interfaces/IBookService.cs
  -> thêm SearchAsync(string? keyword)

backend/LibraryManagement.Application/Services/BookService.cs
  -> implement SearchAsync (keyword rỗng = trả toàn bộ danh sách)

backend/LibraryManagement.API/Controllers/BooksController.cs
  -> GetAll() nhận thêm [FromQuery] string? keyword (SCRUM-37), vẫn yêu cầu Admin/Employee

frontend/library-management-web/pages/catalog.html
  -> viết lại HOÀN TOÀN: bỏ mảng sách hardcode + bỏ chặn "chưa đăng nhập thì redirect login",
     gọi /api/catalog/books thật, có ô tìm kiếm (debounce 350ms) + lọc theo thể loại

frontend/library-management-web/pages/book-detail.html
  -> viết lại HOÀN TOÀN: bỏ object BOOKS hardcode (ISBN/pages/rating giả),
     gọi /api/catalog/books/{id} thật, hiển thị đúng field Book entity + trạng thái
```

## Cách merge

1. Giải nén đè lên project hiện có.
2. **Lưu ý:** `catalog.html` và `book-detail.html` được viết lại hoàn toàn (không phải patch từng
   đoạn) — nếu bạn đã tự chỉnh sửa 2 file này sau Sprint 1, hãy backup trước khi ghi đè.
3. Không có package NuGet mới, không cần `dotnet restore` lại.
4. Không cần thay đổi MongoDB — dùng chung dữ liệu Books/Categories đã seed ở Sprint 2.

## Test nhanh

```bash
# Guest xem danh sách - KHÔNG cần token
curl http://localhost:5000/api/catalog/books

# Member/Guest tìm kiếm
curl "http://localhost:5000/api/catalog/books?keyword=sapiens"

# Xem chi tiết (thay BOOK_ID bằng id thật lấy từ danh sách trên)
curl http://localhost:5000/api/catalog/books/BOOK_ID

# Admin tìm kiếm (cần token)
curl "http://localhost:5000/api/books?keyword=universe" -H "Authorization: Bearer $TOKEN"
```

Mở `frontend/library-management-web/pages/catalog.html` trực tiếp (không cần đăng nhập) để
xem giao diện tìm kiếm + lọc thể loại; bấm "View Details" để sang `book-detail.html`.

## Lưu ý

- Vẫn chưa `dotnet build` được trong sandbox này (không có quyền `nuget.org`) — không có
  package mới ở Sprint 3 nên rủi ro lỗi build thấp hơn Sprint 1/2.
- Trường `CategoryName` trong `CatalogBookResponse` được resolve ở server (join thủ công
  qua Dictionary) để tránh N+1 query và giảm số lần gọi API phía frontend.
