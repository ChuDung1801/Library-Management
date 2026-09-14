using Microsoft.AspNetCore.Mvc;
namespace LibraryManagement.API.Controllers;
[ApiController, Route("api/borrows")]
public class BorrowsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(Array.Empty<object>());
}