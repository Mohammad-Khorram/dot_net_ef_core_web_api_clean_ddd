using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using net_core_web_api_clean_ddd.Domain.Entities;
using net_core_web_api_clean_ddd.Domain.Entities.Auth;
using net_core_web_api_clean_ddd.Domain.Enumerations;
using net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;
using net_core_web_api_clean_ddd.Infrastructure.Generators;
using net_core_web_api_clean_ddd.Infrastructure.Validators;
using net_core_web_api_clean_ddd.Shared;
using net_core_web_api_clean_ddd.Shared.DTOs.Auth.Create;
using net_core_web_api_clean_ddd.Shared.DTOs.Auth.Read;

namespace net_core_web_api_clean_ddd.Infrastructure.Repositories;

public interface IAuthRepository
{
    Task<ApiResponse<string>> SendOtp(SendOtpCreateDto dto, Enumerations.UserRole role, bool isResend);
    Task<ApiResponse<VerifyOtpReadDto>> VerifyOtp(VerifyOtpCreateDto dto, Enumerations.UserRole role);
    Task<ApiResponse<RefreshTokenReadDto>> RefreshToken(RefreshTokenCreateDto dto, Enumerations.UserRole role);
}

public class AuthRepository(DatabaseContext database, IOptions<JwtSettings> jwt) : IAuthRepository
{
    private DatabaseContext Database { get; } = database;
    private JwtSettings Jwt { get; } = jwt.Value;

    public async Task<ApiResponse<string>> SendOtp(SendOtpCreateDto dto, Enumerations.UserRole role, bool isResend)
    {
        if (!MobileNumberValidator.IsValid(dto.PhoneNumber))
            return new BadRequestResponse<string>(error: "شماره موبایل وارد شده معتبر نیست.");

        Enumerations.IssuerType issuerType = role == Enumerations.UserRole.User
            ? Enumerations.IssuerType.MobileApp
            : Enumerations.IssuerType.AdminPanel;

        List<OtpEntity> allOtps = await Database.Otps
            .Where(otp => otp.PhoneNumber == dto.PhoneNumber && otp.IsVerified == false &&
                          otp.Issuer == issuerType.GetDisplayName())
            .OrderByDescending(otp => otp.CreatedDateTime)
            .ToListAsync();

        OtpEntity? lastOtp = allOtps.FirstOrDefault();
        int countInLast10Minutes =
            allOtps.Count(otp => DateTime.Now < otp.CreatedDateTime.GetValueOrDefault().AddMinutes(10));

        if (countInLast10Minutes >= 3)
        {
            DateTime waitUntil = lastOtp?.CreatedDateTime.GetValueOrDefault().AddMinutes(10) ??
                                 DateTime.Now.AddMinutes(10);
            return new BadRequestResponse<string>(
                error:
                $"تعداد درخواست‌های شما بیش از حد مجاز است. لطفاً تا ساعت {waitUntil:HH:mm} صبر کرده و سپس دوباره تلاش کنید.");
        }

        if (lastOtp != null && lastOtp.Attempts >= 3 &&
            DateTime.Now < lastOtp.CreatedDateTime.GetValueOrDefault().AddMinutes(10))
        {
            DateTime waitUntil = lastOtp.CreatedDateTime.GetValueOrDefault().AddMinutes(10);
            return new BadRequestResponse<string>(
                error:
                $"تعداد تلاش‌های شما بیش از حد مجاز است. لطفاً تا ساعت {waitUntil:HH:mm} صبر کرده و سپس دوباره تلاش کنید.");
        }

        string otpCode = await OtpCodeGenerator.Generate(Database.Otps, dto.PhoneNumber);
        string hash = HashGenerator.Generate();

        // Send sms

        OtpEntity otpEntity = new()
        {
            RecId = "Aa123456", // sms_result_id
            PhoneNumber = dto.PhoneNumber,
            OtpCode = otpCode,
            Hash = hash,
            Issuer = issuerType.GetDisplayName(),
            IsResend = isResend
        };

        EntityEntry<OtpEntity> entry = await Database.Otps.AddAsync(otpEntity);
        await Database.SaveChangesAsync();

        // remove phone, code, recId, and issuer in production
        return new ApiResponse<string>
        (response:
            $"phone: {entry.Entity.PhoneNumber}\ncode: {entry.Entity.OtpCode}\nrecId: {entry.Entity.RecId}\nhash: {entry.Entity.Hash}\nissuer: {entry.Entity.Issuer}");
    }

    public async Task<ApiResponse<VerifyOtpReadDto>> VerifyOtp(VerifyOtpCreateDto dto, Enumerations.UserRole role)
    {
        bool isMobileValid = MobileNumberValidator.IsValid(dto.PhoneNumber);
        bool isOtpValid = OtpCodeValidator.IsValid(dto.OtpCode);

        if (!isMobileValid && !isOtpValid)
            return new BadRequestResponse<VerifyOtpReadDto>(
                error: "شماره موبایل و یا رمز یک\u200cبار مصرف وارد شده معتبر نیست.");

        if (!isMobileValid)
            return new BadRequestResponse<VerifyOtpReadDto>(error: "شماره موبایل وارد شده معتبر نیست.");

        if (!isOtpValid)
            return new BadRequestResponse<VerifyOtpReadDto>(error: "رمز یک\u200cبار مصرف وارد شده معتبر نیست.");

        Enumerations.IssuerType issuerType = role == Enumerations.UserRole.User
            ? Enumerations.IssuerType.MobileApp
            : Enumerations.IssuerType.AdminPanel;

        Enumerations.AudienceType audience = role == Enumerations.UserRole.User
            ? Enumerations.AudienceType.MobileUsers
            : Enumerations.AudienceType.AdminUsers;

        List<OtpEntity> otpsByPhone = await Database.Otps
            .Where(otp =>
                otp.PhoneNumber == dto.PhoneNumber && otp.Issuer == issuerType.GetDisplayName())
            .ToListAsync();

        if (otpsByPhone.Count == 0)
            return new BadRequestResponse<VerifyOtpReadDto>(
                error: "کد وارد شده معتبر نیست، لطفاً از طریق دکمه ارسال مجدد کد جدید دریافت کنید.");

        OtpEntity? otpEntry = otpsByPhone
            .FirstOrDefault(otp => otp.OtpCode == dto.OtpCode);

        if (otpEntry == null)
        {
            OtpEntity? lastOtp = otpsByPhone.OrderByDescending(otp => otp.CreatedDateTime).FirstOrDefault();

            if (lastOtp == null)
                return new BadRequestResponse<VerifyOtpReadDto>(
                    error: "کد وارد شده معتبر نیست، لطفاً از طریق دکمه ارسال مجدد کد جدید دریافت کنید.");

            if (lastOtp.Attempts >= 3)
            {
                DateTime waitUntil = lastOtp.CreatedDateTime.GetValueOrDefault().AddMinutes(10);
                return new BadRequestResponse<VerifyOtpReadDto>(
                    error:
                    $"تعداد درخواست‌های شما بیش از حد مجاز است. لطفاً تا ساعت {waitUntil:HH:mm} صبر کرده و سپس دوباره تلاش کنید.");
            }

            lastOtp.Attempts += 1;
            await Database.SaveChangesAsync();
            return new BadRequestResponse<VerifyOtpReadDto>(error: "کد وارد شده معتبر نیست، لطفاً دوباره تلاش کنید.");
        }

        if (otpEntry.IsVerified.GetValueOrDefault())
        {
            otpEntry.Attempts += 1;
            await Database.SaveChangesAsync();
            return new BadRequestResponse<VerifyOtpReadDto>(
                error: "کد وارد شده معتبر نیست، لطفاً از طریق دکمه ارسال مجدد کد جدید دریافت کنید.");
        }

        if (DateTime.Now > otpEntry.ExpirationDateTime)
        {
            otpEntry.Attempts += 1;
            await Database.SaveChangesAsync();
            return new BadRequestResponse<VerifyOtpReadDto>(
                error: "کد وارد شده منقضی شده است، لطفاً از طریق دکمه ارسال مجدد کد جدید دریافت کنید.");
        }

        otpEntry.IsVerified = true;

        // Check user exist in db or not
        UserEntity? existingUser =
            await Database.Users.FirstOrDefaultAsync(user =>
                user.PhoneNumber == dto.PhoneNumber && user.Role == role.GetDisplayName());
        EntityEntry<UserEntity> userEntry;

        // If user not exist in db, add to db (role is important)
        if (existingUser == null)
        {
            UserEntity userEntity = new()
            {
                Role = role.GetDisplayName(),
                PhoneNumber = dto.PhoneNumber
            };
            userEntry = await Database.Users.AddAsync(userEntity);
        }
        else
            userEntry = Database.Users.Entry(existingUser);

        RefreshTokenEntity refreshTokenEntity = new()
        {
            UserId = userEntry.Entity.Id,
            RefreshToken = RefreshTokenGenerator.Generate(),
            Issuer = issuerType.GetDisplayName()
        };

        EntityEntry<RefreshTokenEntity> refreshTokenEntry = await Database.RefreshTokens.AddAsync(refreshTokenEntity);
        await Database.SaveChangesAsync();

        VerifyOtpReadDto response = new()
        {
            Id = userEntry.Entity.Id,
            PhoneNumber = userEntry.Entity.PhoneNumber,
            AccessToken = TokenGenerator.Generate(Jwt, userEntry.Entity.Id, role.GetDisplayName(),
                issuerType.GetDisplayName(),
                audience.GetDisplayName()),
            RefreshToken = refreshTokenEntry.Entity.RefreshToken
        };

        return new ApiResponse<VerifyOtpReadDto>(response: response);
    }

    public async Task<ApiResponse<RefreshTokenReadDto>> RefreshToken(RefreshTokenCreateDto dto,
        Enumerations.UserRole role)
    {
        Enumerations.IssuerType issuerType = role == Enumerations.UserRole.User
            ? Enumerations.IssuerType.MobileApp
            : Enumerations.IssuerType.AdminPanel;

        Enumerations.AudienceType audience = role == Enumerations.UserRole.User
            ? Enumerations.AudienceType.MobileUsers
            : Enumerations.AudienceType.AdminUsers;

        RefreshTokenEntity? refreshTokenEntity = await Database.RefreshTokens
            .FirstOrDefaultAsync(refresh =>
                refresh.RefreshToken == dto.RefreshToken &&
                refresh.Issuer == issuerType.GetDisplayName());
        if (refreshTokenEntity == null || refreshTokenEntity.IsExpired || !refreshTokenEntity.IsActive)
            return new UnauthorizedResponse<RefreshTokenReadDto>();

        UserEntity? userEntity = await Database.Users.FindAsync(dto.UserId);
        if (userEntity == null)
            return new UnauthorizedResponse<RefreshTokenReadDto>();

        // Revoke old refresh token
        refreshTokenEntity.RevokedDateTime = DateTime.Now;

        RefreshTokenEntity newRefreshToken = new()
        {
            UserId = dto.UserId,
            RefreshToken = RefreshTokenGenerator.Generate(),
            Issuer = issuerType.GetDisplayName()
        };

        EntityEntry<RefreshTokenEntity> entry = await Database.RefreshTokens.AddAsync(newRefreshToken);
        await Database.SaveChangesAsync();

        string accessToken = TokenGenerator.Generate(Jwt, dto.UserId, role.GetDisplayName(),
            issuerType.GetDisplayName(),
            audience.GetDisplayName());

        RefreshTokenReadDto response = new()
        {
            AccessToken = accessToken,
            RefreshToken = entry.Entity.RefreshToken
        };

        return new ApiResponse<RefreshTokenReadDto> { Response = response };
    }
}