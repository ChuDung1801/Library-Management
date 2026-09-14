using LibraryManagement.Infrastructure.Data;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>() ?? new MongoDbSettings();
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
builder.Services.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase(mongoSettings.DatabaseName));
builder.Services.AddSingleton<LibraryDbContext>();

var app = builder.Build();
await SeedData.InitializeAsync(app.Services.GetRequiredService<LibraryDbContext>());
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();