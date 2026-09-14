namespace LibraryManagement.Infrastructure.Data;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "LibraryManagement";
}