using Microsoft.AspNetCore.Mvc;
namespace LibraryManagement.API.Controllers;
[ApiController, Route("api/categories")]
public class CategoriesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(Array.Empty<object>());
}