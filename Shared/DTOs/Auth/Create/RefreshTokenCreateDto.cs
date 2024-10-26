namespace net_core_web_api_clean_ddd.Shared.DTOs.Auth.Create;

public class RefreshTokenCreateDto
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}