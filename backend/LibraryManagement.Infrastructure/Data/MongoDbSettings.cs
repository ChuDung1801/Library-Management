namespace LibraryManagement.Infrastructure.Data;

/// <summary>Binding cho section "MongoDbSettings" trong appsettings.json.</summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "LibraryManagementDb";
}
