using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using net_core_web_api_clean_ddd.Application.Admin.DTOs.Categories;
using net_core_web_api_clean_ddd.Domain.Entities;
using net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;
using net_core_web_api_clean_ddd.Infrastructure.Mappers;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Categories;

namespace net_core_web_api_clean_ddd.Infrastructure.Repositories;

public interface ICategoryRepository
{
    Task<ApiResponse<CategoryReadDto>> CreateCategory(CategoryCreateDto dto);
    ApiResponse<IEnumerable<CategoryReadDto>> ReadCategories();
    Task<ApiResponse<CategoryReadDto>> UpdateCategory(CategoryUpdateDto dto);
    Task<ApiResponse<string>> DeleteCategory(Guid id);
}

public class CategoryRepository(DatabaseContext database, ICategoryMapper mapper) : ICategoryRepository
{
    private DatabaseContext Database { get; } = database;
    private ICategoryMapper Mapper { get; } = mapper;

    public async Task<ApiResponse<CategoryReadDto>> CreateCategory(CategoryCreateDto dto)
    {
        CategoryEntity categoryEntity = new()
        {
            Title = dto.Title
        };

        // Create and save
        EntityEntry<CategoryEntity> entity = await Database.Categories.AddAsync(categoryEntity);
        await Database.SaveChangesAsync();

        // Retrieve created entity
        CategoryReadDto response = Mapper.CategoryFromModelToRead(entity.Entity);
        return new ApiResponse<CategoryReadDto>(response: response);
    }

    public ApiResponse<IEnumerable<CategoryReadDto>> ReadCategories()
    {
        // Select query to find list
        IEnumerable<CategoryReadDto> response = Database.Categories
            .AsNoTracking()
            .Select(category =>
                new CategoryReadDto
                {
                    Id = category.Id,
                    Title = category.Title
                })
            .ToList();

        return new ApiResponse<IEnumerable<CategoryReadDto>>(response: response);
    }

    public async Task<ApiResponse<CategoryReadDto>> UpdateCategory(CategoryUpdateDto dto)
    {
        // Find by id
        CategoryEntity? oldCategory = await Database.Categories.FindAsync(dto.Id);

        // Throw Status404NotFound
        if (oldCategory == null)
            return new NotFoundResponse<CategoryReadDto>(error: "Category not found!");

        // Update and save
        oldCategory.Title = dto.Title;
        await Database.SaveChangesAsync();

        // Retrieve updated entity
        CategoryReadDto response = Mapper.CategoryFromUpdateToRead(dto);
        return new ApiResponse<CategoryReadDto>(response: response);
    }

    public async Task<ApiResponse<string>> DeleteCategory(Guid id)
    {
        // Find by id
        CategoryEntity? categoryEntity = await Database.Categories.FindAsync(id);

        // Throw Status404NotFound
        if (categoryEntity == null)
            return new NotFoundResponse<string>(error: "Category not found!");

        // Delete and save
        Database.Remove(categoryEntity);
        await Database.SaveChangesAsync();

        return new ApiResponse<string>(response: "Category has been successfully deleted!");
    }
}