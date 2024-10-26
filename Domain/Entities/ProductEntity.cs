using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_web_api_clean_ddd.Domain.Entities;

[Table("Products")]
public class ProductEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; } = Guid.NewGuid();

    public CategoryEntity Category { get; init; } = null!;
    [ForeignKey("CategoryId")] public required Guid CategoryId { get; set; }
    [MaxLength(64)] public required string Title { get; set; }
    public required int StockCount { get; set; }
}