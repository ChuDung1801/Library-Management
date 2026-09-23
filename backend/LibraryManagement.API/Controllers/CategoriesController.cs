using LibraryManagement.API.DTOs.Category;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>Quản lý thể loại sách - Epic 02 (SCRUM-35). Chỉ Admin/Employee thao tác.</summary>
[ApiController]
[Route("api/categories")]
[Authorize(Roles = "Admin,Employee")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>Xem danh sách thể loại (phục vụ chọn khi thêm/sửa sách).</summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories.Select(Map).ToList());
    }

    /// <summary>Thêm thể loại sách mới (SCRUM-35).</summary>
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await _categoryService.CreateAsync(new CreateCategoryModel
        {
            Name = request.Name,
            CodePrefix = request.CodePrefix,
            Description = request.Description
        });

        return Ok(Map(category));
    }

    private static CategoryResponse Map(Domain.Entities.Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        CodePrefix = category.CodePrefix,
        Description = category.Description,
        CreatedAt = category.CreatedAt
    };
}
