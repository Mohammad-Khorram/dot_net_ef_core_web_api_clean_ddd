using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_web_api_clean_ddd.Domain.Entities.Auth;

[Table("RefreshTokens")]
public class RefreshTokenEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; } = Guid.NewGuid();

    [MaxLength(16)] public required string Issuer { get; init; }
    public UserEntity User { get; init; } = null!;
    [ForeignKey("UserId")] public required Guid UserId { get; init; }
    [MaxLength(64)] public required string RefreshToken { get; init; }
    public DateTime? ExpiresDateTime { get; } = DateTime.Now.AddMonths(6);
    public DateTime? CreatedDateTime { get; init; } = DateTime.Now;
    public DateTime? RevokedDateTime { get; set; }
    public bool IsExpired => DateTime.Now >= ExpiresDateTime.GetValueOrDefault();
    public bool IsActive => RevokedDateTime == null && !IsExpired;
}