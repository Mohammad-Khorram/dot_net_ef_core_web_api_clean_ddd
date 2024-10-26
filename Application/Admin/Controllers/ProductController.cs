using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_core_web_api_clean_ddd.Application.Admin.DTOs.Products;
using net_core_web_api_clean_ddd.Infrastructure.Repositories;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Products;

namespace net_core_web_api_clean_ddd.Application.Admin.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Admin/Product")]
public class ProductController(IProductRepository repository) : ControllerBase
{
    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpPost]
    [Route("CreateProduct")]
    public async Task<ActionResult<ProductReadDto>> CreateProduct(ProductCreateDto dto)
    {
        ApiResponse<ProductReadDto> response = await repository.CreateProduct(dto);
        return StatusCode(response.StatusCode, response);
    }
    
    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpGet]
    [Route("ReadProducts")]
    public ActionResult<ProductReadDto> ReadProducts(int page, int limit)
    {
        ApiResponse<IEnumerable<ProductReadDto>> response = repository.ReadProducts(page, limit);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpPut]
    [Route("UpdateProduct")]
    public async Task<ActionResult<ProductReadDto>> UpdateProduct(ProductUpdateDto dto)
    {
        ApiResponse<ProductReadDto> response = await repository.UpdateProduct(dto);
        return StatusCode(response.StatusCode, response);
    }

    [Authorize(Roles = "Admin", Policy = "AdminPolicy")]
    [HttpDelete]
    [Route("DeleteProduct/{id:guid}")]
    public async Task<ActionResult<string>> DeleteProduct(Guid id)
    {
        ApiResponse<string> response = await repository.DeleteProduct(id);
        return StatusCode(response.StatusCode, response);
    }
}