using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_web_api_clean_ddd.Domain.Entities;

[Table("Users")]
public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; } = Guid.NewGuid();

    [MaxLength(16)] public required string Role { get; set; }
    [MaxLength(11)] public required string PhoneNumber { get; set; }
}