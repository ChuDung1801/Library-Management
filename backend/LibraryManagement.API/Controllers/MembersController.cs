using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Infrastructure.Data;
using MongoDB.Driver;
namespace LibraryManagement.API.Controllers;
[ApiController, Route("api/members")]
public class MembersController(LibraryDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await context.Members.Find(Builders<LibraryManagement.Domain.Entities.Member>.Filter.Empty).ToListAsync());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, MemberUpdateRequest request)
    {
        var update = Builders<LibraryManagement.Domain.Entities.Member>.Update
            .Set(member => member.FullName, request.FullName)
            .Set(member => member.IsActive, request.IsActive);
        var result = await context.Members.UpdateOneAsync(member => member.Id == id, update);
        return result.MatchedCount == 0 ? NotFound() : Ok(await context.Members.Find(member => member.Id == id).FirstOrDefaultAsync());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await context.Members.DeleteOneAsync(member => member.Id == id);
        return result.DeletedCount == 0 ? NotFound() : NoContent();
    }
}

public sealed record MemberUpdateRequest(string FullName, bool IsActive = true);