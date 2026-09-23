# Sprint 2 — Epic 02: Book Management

Zip này **chỉ chứa các file mới/thay đổi của Sprint 2**, giữ nguyên đường dẫn tương đối
so với gốc project (`backend/...`, `frontend/...`, `docs/...`) để bạn copy đè trực tiếp
vào project Sprint 1 đã có.

## User Story hoàn thành

| Story | Endpoint |
|---|---|
| SCRUM-19 Thêm sách mới | `POST /api/books` |
| SCRUM-33 Cập nhật thông tin sách | `PUT /api/books/{id}` |
| SCRUM-34 Xóa sách | `DELETE /api/books/{id}` |
| SCRUM-35 Thêm thể loại sách mới | `POST /api/categories` |
| SCRUM-36 Xem danh sách tất cả sách | `GET /api/books` |

Toàn bộ endpoint Book/Category **chỉ dành cho Admin/Employee** (`[Authorize(Roles = "Admin,Employee")]`) —
đúng phạm vi Sprint 2, chưa mở tìm kiếm/xem công khai cho Member/Guest (việc đó thuộc Sprint 3).

## File mới

```
backend/LibraryManagement.Domain/Enums/BookStatus.cs
backend/LibraryManagement.Domain/Entities/Category.cs
backend/LibraryManagement.Domain/Entities/Book.cs
backend/LibraryManagement.Infrastructure/Repositories/Interfaces/IBookRepository.cs
backend/LibraryManagement.Infrastructure/Repositories/Implementations/BookRepository.cs
backend/LibraryManagement.Infrastructure/Repositories/Interfaces/ICategoryRepository.cs
backend/LibraryManagement.Infrastructure/Repositories/Implementations/CategoryRepository.cs
backend/LibraryManagement.Infrastructure/Data/BookSeedModel.cs
backend/LibraryManagement.Infrastructure/Data/SeedFiles/books.seed.json
backend/LibraryManagement.Application/Interfaces/IBookService.cs
backend/LibraryManagement.Application/Interfaces/ICategoryService.cs
backend/LibraryManagement.Application/Validators/BookValidator.cs
backend/LibraryManagement.Application/Validators/CategoryValidator.cs
backend/LibraryManagement.Application/Services/BookService.cs
backend/LibraryManagement.Application/Services/CategoryService.cs
backend/LibraryManagement.API/DTOs/Book/BookRequests.cs
backend/LibraryManagement.API/DTOs/Book/BookResponse.cs
backend/LibraryManagement.API/DTOs/Category/CategoryDtos.cs
backend/LibraryManagement.API/Controllers/BooksController.cs
backend/LibraryManagement.API/Controllers/CategoriesController.cs
frontend/library-management-web/pages/books.html
frontend/library-management-web/pages/categories.html
docs/test-cases/sprint2-book-management.md
```

## File thay đổi (ghi đè file cũ trong project Sprint 1)

```
backend/LibraryManagement.Infrastructure/Data/LibraryDbContext.cs
  -> thêm 2 collection: Books, Categories

backend/LibraryManagement.Infrastructure/Data/SeedDataService.cs
  -> tách SeedUsersAsync (giữ nguyên logic Sprint 1) + thêm SeedBooksAsync (books.json)

backend/LibraryManagement.API/Program.cs
  -> đăng ký DI: IBookRepository, ICategoryRepository, IBookService, ICategoryService

frontend/library-management-web/js/books.js
  -> từ file stub rỗng, đã có BooksApi (getAll/getById/create/update/remove)

frontend/library-management-web/js/categories.js
  -> từ file stub rỗng, đã có CategoriesApi (getAll/create)

frontend/library-management-web/pages/home.html
  -> sidebar "Books" giờ là link sang books.html, thêm link Categories,
     nút "+ Add New Resource" trỏ sang books.html
```

## Cách merge

1. Giải nén zip này đè lên thư mục project Sprint 1 (các đường dẫn trùng nhau sẽ merge đúng vị trí).
2. Với 5 file "thay đổi" ở trên — nếu bạn đã tự sửa gì thêm ở Sprint 1 trong các file đó,
   hãy diff thủ công trước khi ghi đè để không mất thay đổi của bạn.
3. `dotnet restore` lại (không có package mới ngoài những gói đã cài ở Sprint 1).
4. MongoDB: `SeedDataService` sẽ tự thêm 12 sách mẫu từ `books.json` vào collection `Books`
   khi collection đang rỗng — không ảnh hưởng dữ liệu `Users` đã seed ở Sprint 1.

## Test nhanh bằng curl

```bash
# Đăng nhập admin để lấy token (xem Sprint 1)
TOKEN="..."

# Thêm thể loại
curl -X POST http://localhost:5000/api/categories \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"name":"Vũ trụ & Thiên văn","codePrefix":"UNIVER"}'

# Thêm sách
curl -X POST http://localhost:5000/api/books \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"bookCode":"UNIVER-0001","title":"The Elegant Universe","author":"Brian Greene","publisher":"Vintage","publicationYear":1999,"totalCopies":5}'

# Xem danh sách sách
curl http://localhost:5000/api/books -H "Authorization: Bearer $TOKEN"
```

## Lưu ý

- Vẫn **chưa build/verify** được bằng `dotnet build` trong sandbox này (không có quyền truy cập
  `nuget.org`). Cấu trúc code tuân thủ chặt chiều dependency `API → Application → Infrastructure → Domain`
  đã thống nhất ở Sprint 1 (không có circular reference mới).
- Mã sách (`bookCode`) bắt buộc nhập tay theo định dạng `PREFIX-0000` (vd `UNIVER-0001`), hệ thống
  **chưa tự sinh mã** — nếu cần tự động sinh theo thể loại, có thể bổ sung ở Sprint sau.
