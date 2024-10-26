using net_core_web_api_clean_ddd.Application.Admin.DTOs.Categories;
using net_core_web_api_clean_ddd.Domain.Entities;
using net_core_web_api_clean_ddd.Shared.DTOs.Categories;

namespace net_core_web_api_clean_ddd.Infrastructure.Mappers;

public interface ICategoryMapper
{
    CategoryReadDto CategoryFromModelToRead(CategoryEntity entity);
    CategoryEntity CategoryFromUpdateToModel(CategoryUpdateDto dto);
    CategoryReadDto CategoryFromUpdateToRead(CategoryUpdateDto dto);
}

public class CategoryMapper : ICategoryMapper
{
    public CategoryReadDto CategoryFromModelToRead(CategoryEntity entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title,
        };

    public CategoryEntity CategoryFromUpdateToModel(CategoryUpdateDto dto) =>
        new()
        {
            Title = dto.Title
        };

    public CategoryReadDto CategoryFromUpdateToRead(CategoryUpdateDto dto) =>
        new()
        {
            Id = dto.Id,
            Title = dto.Title
        };
}