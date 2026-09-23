using System.Text.Json;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Data;

/// <summary>
/// Nạp dữ liệu mẫu vào MongoDB khi khởi động: users.json (Sprint 1) và books.json (Sprint 2),
/// mỗi collection chỉ seed nếu đang rỗng - không ghi đè dữ liệu người dùng đã thao tác.
///
/// LƯU Ý QUAN TRỌNG: IHostedService luôn được đăng ký với lifetime Singleton bởi
/// AddHostedService(), nên KHÔNG được inject thẳng các service Scoped (IUserRepository,
/// IBookRepository...) qua constructor - ASP.NET Core sẽ ném lỗi ngay lúc validate DI:
/// "Cannot consume scoped service ... from singleton ...". Thay vào đó, ta chỉ inject
/// IServiceScopeFactory (Singleton) và tự tạo một scope ngắn hạn bên trong StartAsync
/// để resolve các service Scoped an toàn.
/// </summary>
public class SeedDataService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SeedDataService> _logger;

    public SeedDataService(IServiceScopeFactory scopeFactory, ILogger<SeedDataService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var bookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await SeedUsersAsync(userRepository, passwordHasher, cancellationToken);
        await SeedBooksAsync(bookRepository, cancellationToken);
    }

    private async Task SeedUsersAsync(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken)
    {
        var existingCount = await userRepository.CountAsync();
        if (existingCount > 0)
        {
            _logger.LogInformation("Bỏ qua seed Users: đã có {Count} bản ghi.", existingCount);
            return;
        }

        var seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedFiles", "users.seed.json");
        if (!File.Exists(seedPath))
        {
            _logger.LogWarning("Không tìm thấy file seed: {Path}", seedPath);
            return;
        }

        var json = await File.ReadAllTextAsync(seedPath, cancellationToken);
        var seedUsers = JsonSerializer.Deserialize<List<UserSeedModel>>(json) ?? new List<UserSeedModel>();

        foreach (var seed in seedUsers)
        {
            var (hash, salt) = passwordHasher.HashPassword(seed.MatKhau);

            var role = seed.QuyenHan switch
            {
                "Admin" => UserRole.Admin,
                "Employee" => UserRole.Employee,
                _ => UserRole.Member
            };

            var user = new User
            {
                FullName = seed.Ten,
                Username = seed.TenDangNhap,
                // users.json chưa có email/SĐT thật: sinh giá trị tạm để đáp ứng ràng buộc bắt buộc,
                // người dùng có thể cập nhật lại qua chức năng "Cập nhật thông tin cá nhân" (SCRUM-30).
                Email = $"{seed.TenDangNhap}@scholaris.edu",
                PhoneNumber = "0000000000",
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.CreateAsync(user);
        }

        _logger.LogInformation("Đã seed {Count} tài khoản mẫu vào MongoDB.", seedUsers.Count);
    }

    private async Task SeedBooksAsync(IBookRepository bookRepository, CancellationToken cancellationToken)
    {
        var existingBooks = await bookRepository.GetAllAsync();
        if (existingBooks.Count > 0)
        {
            _logger.LogInformation("Bỏ qua seed Books: đã có {Count} bản ghi.", existingBooks.Count);
            return;
        }

        var seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedFiles", "books.seed.json");
        if (!File.Exists(seedPath))
        {
            _logger.LogWarning("Không tìm thấy file seed: {Path}", seedPath);
            return;
        }

        var json = await File.ReadAllTextAsync(seedPath, cancellationToken);
        var seedBooks = JsonSerializer.Deserialize<List<BookSeedModel>>(json) ?? new List<BookSeedModel>();

        var index = 1;
        foreach (var seed in seedBooks)
        {
            // books.json chưa có Mã sách/Thể loại/Số lượng thật: sinh mã tạm tuần tự
            // (BOOK-0001, BOOK-0002...) và số lượng mặc định, admin có thể cập nhật lại
            // qua chức năng "Cập nhật thông tin sách" (SCRUM-33).
            var book = new Book
            {
                BookCode = $"BOOK-{index:D4}",
                Title = seed.TenSach,
                Author = seed.TacGia,
                Publisher = "Chưa xác định",
                PublicationYear = seed.NamXuatBan,
                Introduction = seed.GioiThieuSach,
                Summary = seed.SoLuocNoiDung,
                TotalCopies = 5,
                CategoryId = null,
                Status = BookStatus.Available,
                CreatedAt = DateTime.UtcNow
            };

            await bookRepository.CreateAsync(book);
            index++;
        }

        _logger.LogInformation("Đã seed {Count} đầu sách mẫu vào MongoDB.", seedBooks.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}