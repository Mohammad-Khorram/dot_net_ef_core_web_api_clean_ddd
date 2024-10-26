using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_core_web_api_clean_ddd.Application.Admin.DTOs.Categories;
using net_core_web_api_clean_ddd.Infrastructure.Repositories;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Categories;

namespace net_core_web_api_clean_ddd.Application.Admin.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Admin/Category")]
public class CategoryController(ICategoryRepository repository) : ControllerBase
{
    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpPost]
    [Route("CreateCategory")]
    public async Task<ActionResult<CategoryReadDto>> CreateCategory(CategoryCreateDto dto)
    {
        ApiResponse<CategoryReadDto> response = await repository.CreateCategory(dto);
        return StatusCode(response.StatusCode, response);
    }
    
    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpGet]
    [Route("ReadCategories")]
    public ActionResult<CategoryReadDto> ReadCategories()
    {
        ApiResponse<IEnumerable<CategoryReadDto>> response = repository.ReadCategories();
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpPut]
    [Route("UpdateCategory")]
    public async Task<ActionResult<CategoryReadDto>> UpdateCategory(CategoryUpdateDto dto)
    {
        ApiResponse<CategoryReadDto> response = await repository.UpdateCategory(dto);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpDelete]
    [Route("DeleteCategory/{id:guid}")]
    public async Task<ActionResult<string>> DeleteCategory(Guid id)
    {
        ApiResponse<string> response = await repository.DeleteCategory(id);
        return StatusCode(response.StatusCode, response);
    }
}