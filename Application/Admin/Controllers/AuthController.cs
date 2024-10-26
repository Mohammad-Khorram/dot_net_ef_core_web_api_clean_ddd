using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using net_core_web_api_clean_ddd.Domain.Enumerations;
using net_core_web_api_clean_ddd.Infrastructure.Repositories;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Auth.Create;
using net_core_web_api_clean_ddd.Shared.DTOs.Auth.Read;

namespace net_core_web_api_clean_ddd.Application.Admin.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Admin/Auth")]
public class AuthController(IAuthRepository repository) : ControllerBase
{
    [HttpPost("SendOtp")]
    public async Task<ActionResult<string>> SendOtp([FromBody] SendOtpCreateDto dto)
    {
        ApiResponse<string> response = await repository.SendOtp(dto, Enumerations.UserRole.Admin, false);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("ResendOtp")]
    public async Task<ActionResult<string>> ResendOtp([FromBody] SendOtpCreateDto dto)
    {
        ApiResponse<string> response = await repository.SendOtp(dto, Enumerations.UserRole.Admin, true);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("VerifyOtp")]
    public async Task<ActionResult<VerifyOtpReadDto>> VerifyOtp([FromBody] VerifyOtpCreateDto dto)
    {
        ApiResponse<VerifyOtpReadDto> response = await repository.VerifyOtp(dto, Enumerations.UserRole.Admin);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<RefreshTokenReadDto>> RefreshToken([FromBody] RefreshTokenCreateDto dto)
    {
        ApiResponse<RefreshTokenReadDto> response = await repository.RefreshToken(dto, Enumerations.UserRole.Admin);
        return StatusCode(response.StatusCode, response);
    }
}