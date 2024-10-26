namespace net_core_web_api_clean_ddd.Shared.DTOs.Auth.Create;

public class VerifyOtpCreateDto
{
    public required string PhoneNumber { get; set; }
    public required string OtpCode { get; set; }
}