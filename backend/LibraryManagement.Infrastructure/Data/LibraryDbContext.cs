using LibraryManagement.Domain.Entities;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Data;

public sealed class LibraryDbContext
{
    public LibraryDbContext(IMongoDatabase database)
    {
        Users = database.GetCollection<User>("users");
        Books = database.GetCollection<Book>("books");
        Members = database.GetCollection<Member>("members");
        Categories = database.GetCollection<Category>("categories");
        Borrows = database.GetCollection<Borrow>("borrows");
        BorrowDetails = database.GetCollection<BorrowDetail>("borrowDetails");
        ExtensionRequests = database.GetCollection<ExtensionRequest>("extensionRequests");
    }

    public IMongoCollection<User> Users { get; }
    public IMongoCollection<Book> Books { get; }
    public IMongoCollection<Member> Members { get; }
    public IMongoCollection<Category> Categories { get; }
    public IMongoCollection<Borrow> Borrows { get; }
    public IMongoCollection<BorrowDetail> BorrowDetails { get; }
    public IMongoCollection<ExtensionRequest> ExtensionRequests { get; }

    public async Task EnsureIndexesAsync()
    {
        await Users.Indexes.CreateManyAsync([
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(user => user.Username), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(user => user.Email), new CreateIndexOptions { Unique = true })
        ]);
        await Books.Indexes.CreateOneAsync(new CreateIndexModel<Book>(Builders<Book>.IndexKeys.Ascending(book => book.Code), new CreateIndexOptions { Unique = true, Sparse = true }));
        await Categories.Indexes.CreateOneAsync(new CreateIndexModel<Category>(Builders<Category>.IndexKeys.Ascending(category => category.Name), new CreateIndexOptions { Unique = true }));
        await Members.Indexes.CreateOneAsync(new CreateIndexModel<Member>(Builders<Member>.IndexKeys.Ascending(member => member.UserId), new CreateIndexOptions { Unique = true, Sparse = true }));
        await BorrowDetails.Indexes.CreateOneAsync(new CreateIndexModel<BorrowDetail>(Builders<BorrowDetail>.IndexKeys.Ascending(detail => detail.BorrowId)));
    }
}