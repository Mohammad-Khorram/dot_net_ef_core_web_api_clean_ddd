namespace net_core_web_api_clean_ddd.Application.Admin.DTOs.Categories;

public class CategoryUpdateDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
}