using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Infrastructure.Data;
using MongoDB.Driver;
namespace LibraryManagement.API.Controllers;
[ApiController, Route("api/books")]
public class BooksController(LibraryDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var filter = string.IsNullOrWhiteSpace(search)
            ? Builders<LibraryManagement.Domain.Entities.Book>.Filter.Empty
            : Builders<LibraryManagement.Domain.Entities.Book>.Filter.Or(
                Builders<LibraryManagement.Domain.Entities.Book>.Filter.Regex(book => book.Title, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                Builders<LibraryManagement.Domain.Entities.Book>.Filter.Regex(book => book.Author, new MongoDB.Bson.BsonRegularExpression(search, "i")));

        return Ok(await context.Books.Find(filter).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author) || book.TotalCopies < 0)
            return BadRequest("Title, author and a non-negative copy count are required.");

        book.Id = Guid.NewGuid();
        book.AvailableCopies = book.TotalCopies;
        await context.Books.InsertOneAsync(book);
        return CreatedAtAction(nameof(GetAll), new { id = book.Id }, book);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author) || book.TotalCopies < 0 || book.AvailableCopies < 0 || book.AvailableCopies > book.TotalCopies)
            return BadRequest("Invalid book data.");

        book.Id = id;
        var result = await context.Books.ReplaceOneAsync(existing => existing.Id == id, book);
        return result.MatchedCount == 0 ? NotFound() : Ok(book);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await context.Books.DeleteOneAsync(book => book.Id == id);
        return result.DeletedCount == 0 ? NotFound() : NoContent();
    }
}