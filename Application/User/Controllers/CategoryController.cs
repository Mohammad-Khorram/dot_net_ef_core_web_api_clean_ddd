using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using net_core_web_api_clean_ddd.Infrastructure.Repositories;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Categories;

namespace net_core_web_api_clean_ddd.Application.User.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Category")]
public class CategoryController(ICategoryRepository repository) : ControllerBase
{
    [HttpGet]
    [Route("ReadCategories")]
    public ActionResult<CategoryReadDto> ReadCategories()
    {
        ApiResponse<IEnumerable<CategoryReadDto>> response = repository.ReadCategories();
        return StatusCode(response.StatusCode, response);
    }
}