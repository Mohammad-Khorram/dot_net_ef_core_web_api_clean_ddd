using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using net_core_web_api_clean_ddd.Application.Admin.DTOs.Products;
using net_core_web_api_clean_ddd.Domain.Entities;
using net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;
using net_core_web_api_clean_ddd.Infrastructure.Mappers;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Products;

namespace net_core_web_api_clean_ddd.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<ApiResponse<ProductReadDto>> CreateProduct(ProductCreateDto dto);
    ApiResponse<IEnumerable<ProductReadDto>> ReadProducts(int page, int limit);
    Task<ApiResponse<ProductReadDto>> UpdateProduct(ProductUpdateDto dto);
    Task<ApiResponse<string>> DeleteProduct(Guid id);
}

public class ProductRepository(DatabaseContext database, IProductMapper mapper) : IProductRepository
{
    private DatabaseContext Database { get; } = database;
    private IProductMapper Mapper { get; } = mapper;

    public async Task<ApiResponse<ProductReadDto>> CreateProduct(ProductCreateDto dto)
    {
        // Find by categoryId
        CategoryEntity? categoryEntity = await Database.Categories.FindAsync(dto.CategoryId);

        // Throw Status404NotFound
        if (categoryEntity == null)
            return new NotFoundResponse<ProductReadDto>(error: "Category not found!");

        ProductEntity productEntity = new()
        {
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            StockCount = dto.StockCount
        };

        // Create and save
        EntityEntry<ProductEntity> entity = await Database.Products.AddAsync(productEntity);
        await Database.SaveChangesAsync();

        // Retrieve created entity
        ProductReadDto response = Mapper.ProductFromModelToRead(entity.Entity);
        return new ApiResponse<ProductReadDto>(response: response);
    }

    public ApiResponse<IEnumerable<ProductReadDto>> ReadProducts(int page, int limit)
    {
        if (page < 1) page = 1;
        if (limit < 1 || limit > 100) limit = 24; // default limit=24

        int totalCount = Database.Products.Count();
        int totalPages = (int)Math.Ceiling((double)totalCount / limit);

        // Select query to find list
        IEnumerable<ProductReadDto> response = Database.Products
            .AsNoTracking()
            .Select(product =>
                new ProductReadDto
                {
                    CategoryId = product.CategoryId,
                    Id = product.Id,
                    Title = product.Title,
                    StockCount = product.StockCount
                })
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToList();

        return new ApiResponse<IEnumerable<ProductReadDto>>(
            totalPages: totalPages,
            page: page,
            limit: limit,
            response: response
        );
    }

    public async Task<ApiResponse<ProductReadDto>> UpdateProduct(ProductUpdateDto dto)
    {
        // Find by id
        ProductEntity? oldProduct = await Database.Products.FindAsync(dto.Id);

        // Throw Status404NotFound
        if (oldProduct == null)
            return new NotFoundResponse<ProductReadDto>(error: "Product not found!");
        
        // Check if the CategoryId is updated or not
        if (oldProduct.CategoryId != dto.CategoryId)
        {
            CategoryEntity? categoryEntity = await Database.Categories
                .Where(category => category.Id == dto.CategoryId)
                .FirstOrDefaultAsync();

            if (categoryEntity == null)
                return new NotFoundResponse<ProductReadDto>(error: "New Category not found!");

            oldProduct.CategoryId = dto.CategoryId;
        }

        // Update and save
        oldProduct.Title = dto.Title;
        oldProduct.StockCount = dto.StockCount;
        await Database.SaveChangesAsync();

        // Retrieve updated entity
        ProductReadDto response = Mapper.ProductFromUpdateToRead(dto);
        return new ApiResponse<ProductReadDto>(response: response);
    }

    public async Task<ApiResponse<string>> DeleteProduct(Guid id)
    {
        // Find by id
        ProductEntity? productEntity = await Database.Products.FindAsync(id);

        // Throw Status404NotFound
        if (productEntity == null)
            return new NotFoundResponse<string>(error: "Product not found!");

        // Delete and save
        Database.Remove(productEntity);
        await Database.SaveChangesAsync();

        return new ApiResponse<string>(response: "Product has been successfully deleted!");
    }
}