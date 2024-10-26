using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_core_web_api_clean_ddd.Domain.Entities.Auth;

[Table("Otps")]
public class OtpEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; } = Guid.NewGuid();

    [MaxLength(16)] public required string Issuer { get; init; }
    [MaxLength(32)] public required string RecId { get; init; }
    [MaxLength(11)] public required string PhoneNumber { get; init; }
    [MaxLength(5)] public required string OtpCode { get; init; }
    [MaxLength(11)] public required string Hash { get; init; }
    public bool? IsVerified { get; set; } = false;
    public int? Attempts { get; set; } = 0;
    public bool? IsResend { get; init; } = false;
    public DateTime? ExpirationDateTime { get; init; } = DateTime.Now.AddMinutes(2);
    public DateTime? CreatedDateTime { get; init; } = DateTime.Now;
}