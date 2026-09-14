using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(LibraryDbContext context)
    {
        await context.EnsureIndexesAsync();

        var categoryId = Guid.Parse("d1d7c7e1-8c10-4c8f-9c1e-000000000001");
        var programmingCategoryId = Guid.Parse("d1d7c7e1-8c10-4c8f-9c1e-000000000002");
        var adminUserId = Guid.Parse("a1a1a1a1-1111-4111-8111-000000000001");
        var memberUserId = Guid.Parse("a1a1a1a1-1111-4111-8111-000000000002");
        var memberId = Guid.Parse("8a0e6b54-3d89-4be2-8b8e-2c5b2e9a0001");

        var categories = new[]
        {
            new Category { Id = categoryId, Name = "Công nghệ", Description = "Sách công nghệ và kỹ thuật." },
            new Category { Id = programmingCategoryId, Name = "Lập trình", Description = "Sách về phát triển phần mềm." }
        };

        var users = new[]
        {
            new User
            {
                Id = adminUserId,
                FullName = "Library Administrator",
                Username = "admin",
                Email = "admin@library.local",
                PhoneNumber = "0900000000",
                Role = Role.Admin,
                PasswordHash = "seed-password-change-me"
            },
            new User
            {
                Id = memberUserId,
                FullName = "Nguyen Van An",
                Username = "nguyenvanan",
                Email = "nguyenvanan@library.local",
                PhoneNumber = "0911111111",
                Role = Role.Member,
                PasswordHash = "seed-password-change-me"
            }
        };

        var books = new[]
        {
            new Book
            {
                Id = Guid.Parse("6f5c2d39-5f3c-4e3c-9cc5-7f6e7b9f0001"),
                Code = "CLEAN-0001",
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Publisher = "Prentice Hall",
                PublicationYear = 2008,
                Description = "Nguyên tắc viết mã nguồn dễ đọc và bảo trì.",
                Summary = "Các nguyên tắc thực hành để cải thiện chất lượng code.",
                CategoryIds = [categoryId, programmingCategoryId],
                TotalCopies = 3,
                AvailableCopies = 3
            },
            new Book
            {
                Id = Guid.Parse("6f5c2d39-5f3c-4e3c-9cc5-7f6e7b9f0002"),
                Code = "PRAG-0001",
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt and David Thomas",
                Publisher = "Addison-Wesley",
                PublicationYear = 1999,
                Description = "Kinh nghiệm và nguyên tắc phát triển phần mềm thực dụng.",
                Summary = "Hướng dẫn tư duy và kỹ năng của lập trình viên chuyên nghiệp.",
                CategoryIds = [programmingCategoryId],
                TotalCopies = 2,
                AvailableCopies = 1
            },
            new Book
            {
                Id = Guid.Parse("6f5c2d39-5f3c-4e3c-9cc5-7f6e7b9f0003"),
                Code = "DATA-0001",
                Title = "Designing Data-Intensive Applications",
                Author = "Martin Kleppmann",
                Publisher = "O'Reilly Media",
                PublicationYear = 2017,
                Description = "Thiết kế các ứng dụng dữ liệu có khả năng mở rộng.",
                Summary = "Kiến trúc, lưu trữ và xử lý dữ liệu hiện đại.",
                CategoryIds = [categoryId],
                TotalCopies = 2,
                AvailableCopies = 2
            }
        };

        var members = new[]
        {
            new Member
            {
                Id = memberId,
                UserId = memberUserId,
                FullName = "Nguyen Van An",
                PhoneNumber = "0911111111",
                Email = "nguyenvanan@library.local",
                IsActive = true
            },
            new Member
            {
                Id = Guid.Parse("8a0e6b54-3d89-4be2-8b8e-2c5b2e9a0002"),
                UserId = Guid.Parse("a1a1a1a1-1111-4111-8111-000000000003"),
                FullName = "Tran Thi Binh",
                PhoneNumber = "0922222222",
                Email = "tranthibinh@library.local",
                IsActive = true
            }
        };

        foreach (var category in categories)
            await context.Categories.ReplaceOneAsync(existing => existing.Id == category.Id, category, new ReplaceOptions { IsUpsert = true });

        foreach (var user in users)
            await context.Users.ReplaceOneAsync(existing => existing.Id == user.Id, user, new ReplaceOptions { IsUpsert = true });

        foreach (var book in books)
        {
            await context.Books.ReplaceOneAsync(
                Builders<Book>.Filter.Eq(existing => existing.Id, book.Id),
                book,
                new ReplaceOptions { IsUpsert = true });
        }

        foreach (var member in members)
        {
            await context.Members.ReplaceOneAsync(
                Builders<Member>.Filter.Eq(existing => existing.Id, member.Id),
                member,
                new ReplaceOptions { IsUpsert = true });
        }

        var borrow = new Borrow
        {
            Id = Guid.Parse("b1b1b1b1-2222-4222-8222-000000000001"),
            MemberId = memberId,
            BorrowedAt = DateTime.UtcNow.AddDays(-5),
            DueAt = DateTime.UtcNow.AddDays(25),
            Status = BorrowStatus.Active
        };
        await context.Borrows.ReplaceOneAsync(existing => existing.Id == borrow.Id, borrow, new ReplaceOptions { IsUpsert = true });
        await context.BorrowDetails.ReplaceOneAsync(
            existing => existing.Id == Guid.Parse("b2b2b2b2-3333-4333-8333-000000000001"),
            new BorrowDetail
            {
                Id = Guid.Parse("b2b2b2b2-3333-4333-8333-000000000001"),
                BorrowId = borrow.Id,
                BookId = Guid.Parse("6f5c2d39-5f3c-4e3c-9cc5-7f6e7b9f0002"),
                Quantity = 1
            },
            new ReplaceOptions { IsUpsert = true });
    }
}