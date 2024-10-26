using net_core_web_api_clean_ddd.Domain.Entities;
using net_core_web_api_clean_ddd.Shared.DTOs.Products;
using net_core_web_api_clean_ddd.Application.Admin.DTOs.Products;

namespace net_core_web_api_clean_ddd.Infrastructure.Mappers;

public interface IProductMapper
{
    ProductReadDto ProductFromModelToRead(ProductEntity entity);
    ProductEntity ProductFromUpdateToModel(ProductUpdateDto dto);
    ProductReadDto ProductFromUpdateToRead(ProductUpdateDto dto);
}

public class ProductMapper : IProductMapper
{
    public ProductReadDto ProductFromModelToRead(ProductEntity entity) =>
        new()
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            Title = entity.Title,
            StockCount = entity.StockCount
        };

    public ProductEntity ProductFromUpdateToModel(ProductUpdateDto dto) =>
        new()
        {
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            StockCount = dto.StockCount
        };

    public ProductReadDto ProductFromUpdateToRead(ProductUpdateDto dto) =>
        new()
        {
            Id = dto.Id,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            StockCount = dto.StockCount
        };
}