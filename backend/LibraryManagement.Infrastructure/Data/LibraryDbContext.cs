using LibraryManagement.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Data;

/// <summary>
/// Điểm truy cập duy nhất tới MongoDB. Mỗi Sprint sẽ bổ sung thêm collection tương ứng
/// (Books, Categories, Borrows, ...). Sprint 1 chỉ cần collection Users.
/// </summary>
public class LibraryDbContext
{
    private readonly IMongoDatabase _database;

    public LibraryDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Book> Books => _database.GetCollection<Book>("Books");
    public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
    public IMongoCollection<Borrow> Borrows => _database.GetCollection<Borrow>("Borrows");
    public IMongoCollection<BorrowRequest> BorrowRequests => _database.GetCollection<BorrowRequest>("BorrowRequests");
}
