namespace net_core_web_api_clean_ddd.Shared.DTOs.Auth.Read;

public class VerifyOtpReadDto
{
    public required Guid Id { get; set; }
    public required string PhoneNumber { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}