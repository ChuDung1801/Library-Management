# Folder Structure – Hệ thống quản lý thư viện

Cấu trúc thư mục được thiết kế dựa trên SKILL Agent của dự án: hệ thống quản lý sách, thể loại, thành viên và nghiệp vụ mượn/trả; ưu tiên đơn giản, dễ triển khai cho nhóm 3 người, đồng thời tách Frontend – Backend – Database rõ ràng.

## 1. Cấu trúc tổng thể

Frontend của dự án chỉ sử dụng **HTML/CSS/JavaScript cơ bản**, không sử dụng React hoặc framework Frontend.

```text
LibraryManagement/
│
├── LibraryManagement.sln
├── README.md
├── SKILL_LM.md
├── FolderContruct.md
│
├── backend/
│   ├── LibraryManagement.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── BooksController.cs
│   │   │   ├── CategoriesController.cs
│   │   │   ├── MembersController.cs
│   │   │   └── BorrowsController.cs
│   │   │
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Book/
│   │   │   ├── Category/
│   │   │   ├── Member/
│   │   │   └── Borrow/
│   │   │
│   │   ├── Middleware/
│   │   │   └── ExceptionMiddleware.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── LibraryManagement.Application/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   │
│   ├── LibraryManagement.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Exceptions/
│   │
│   └── LibraryManagement.Infrastructure/
│       ├── Data/
│       │   ├── LibraryDbContext.cs
│       │   └── Configurations/
│       ├── Repositories/
│       │   └── Interfaces/
│       │   └── Implementations/
│       └── Migrations/
│
├── frontend/
│   └── library-management-web/
│       ├── index.html
│       │
│       ├── pages/
│       │   ├── login.html
│       │   ├── register.html
│       │   ├── books.html
│       │   ├── book-detail.html
│       │   ├── categories.html
│       │   ├── members.html
│       │   ├── borrows.html
│       │   ├── borrow-history.html
│       │   └── overdue-books.html
│       │
│       ├── css/
│       │   ├── style.css
│       │   ├── auth.css
│       │   └── admin.css
│       │
│       ├── js/
│       │   ├── api.js
│       │   ├── auth.js
│       │   ├── books.js
│       │   ├── categories.js
│       │   ├── members.js
│       │   └── borrows.js
│       │
│       └── assets/
│           ├── images/
│           └── icons/
│
├── tests/
│   ├── LibraryManagement.UnitTests/
│   └── LibraryManagement.IntegrationTests/
│
└── docs/
    ├── diagrams/
    │   ├── UseCase/
    │   ├── Activity/
    │   ├── Sequence/
    │   └── ERD/
    ├── api/
    └── test-cases/
```

## 2. Phân chia trách nhiệm

### Domain

Chứa nghiệp vụ và các entity cốt lõi, không phụ thuộc vào API hay giao diện.

- `Entities/`: User, Member, Book, Category, Borrow, BorrowDetail, ExtensionRequest.
- `Enums/`: Role, trạng thái sách và trạng thái mượn.
- `Exceptions/`: các lỗi nghiệp vụ.

### Application

Chứa logic xử lý nghiệp vụ của hệ thống.

- `Interfaces/`: định nghĩa các service.
- `Services/`: xử lý CRUD và nghiệp vụ mượn/trả.
- `Validators/`: kiểm tra dữ liệu đầu vào.

### Infrastructure

Chịu trách nhiệm làm việc với cơ sở dữ liệu và các thành phần bên ngoài.

- `Data/`: DbContext và cấu hình EF Core.
- `Repositories/`: truy xuất dữ liệu.
- `Migrations/`: migration của database.

### API

Là tầng giao tiếp với Frontend.

- `Controllers/`: cung cấp REST API.
- `DTOs/`: dữ liệu request/response.
- `Middleware/`: xử lý lỗi dùng chung.
- `Program.cs`: cấu hình ứng dụng và Dependency Injection.

### Frontend

Cung cấp giao diện cho Guest, Member và Admin.

Các màn hình chính tương ứng với Product Backlog:

- Authentication.
- Tìm kiếm/xem sách.
- CRUD sách.
- CRUD thể loại.
- Quản lý thành viên.
- Mượn/trả sách.
- Lịch sử mượn.
- Theo dõi sách quá hạn.
- Gia hạn sách.

### Tests

Tách test theo hai mức chính:

- `UnitTests/`: kiểm thử service và validation.
- `IntegrationTests/`: kiểm thử API và luồng tích hợp.

### Docs

Lưu tài liệu thiết kế và kiểm thử:

- Use Case.
- Activity Diagram.
- Sequence Diagram.
- ERD.
- API documentation.
- Test Case.

## 3. Nguyên tắc tổ chức

Cấu trúc này ưu tiên kiến trúc nhiều tầng nhưng vẫn giữ mức đơn giản phù hợp với dự án nhóm nhỏ.

Luồng xử lý chính:

```text
Frontend
   ↓
Controller
   ↓
Application Service
   ↓
Repository
   ↓
Infrastructure / EF Core
   ↓
Database
```

Dependency nên đi theo hướng:

```text
API → Application → Domain
             ↓
      Infrastructure
```

Trong đó:

- Controller không xử lý trực tiếp nghiệp vụ phức tạp.
- Service chịu trách nhiệm xử lý nghiệp vụ.
- Repository chịu trách nhiệm truy cập dữ liệu.
- Entity và quy tắc nghiệp vụ cốt lõi nằm ở Domain.
- DTO dùng để tránh đưa trực tiếp Entity ra API.
- Dependency Injection được sử dụng để giảm phụ thuộc giữa các tầng.

## 4. Phạm vi CRUD chính

| Module | CRUD chính |
|---|---|
| Books | Create, Read, Update, Delete, Search |
| Categories | Create, Read, Update, Delete |
| Members | Read, Update, quản lý tài khoản |
| Borrows | Create, Read, Update trạng thái trả |
| Borrow Details | Quản lý sách trong phiếu mượn |
| Extension Request | Create, Read, xử lý yêu cầu gia hạn |

Cấu trúc không bổ sung các module lớn ngoài phạm vi Product Backlog. Các chức năng nâng cao chỉ nên được thêm sau khi toàn bộ Must Have đã hoàn thành.
