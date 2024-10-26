using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using net_core_web_api_clean_ddd.Infrastructure.Repositories;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Products;

namespace net_core_web_api_clean_ddd.Application.User.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Product")]
public class ProductController(IProductRepository repository) : ControllerBase
{
    [HttpGet]
    [Route("ReadProducts")]
    public ActionResult<ProductReadDto> ReadProducts(int page, int limit)
    {
        ApiResponse<IEnumerable<ProductReadDto>> response = repository.ReadProducts(page, limit);
        return StatusCode(response.StatusCode, response);
    }
}