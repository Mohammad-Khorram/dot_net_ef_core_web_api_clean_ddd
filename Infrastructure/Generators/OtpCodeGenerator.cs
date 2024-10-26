using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using net_core_web_api_clean_ddd.Domain.Entities.Auth;

namespace net_core_web_api_clean_ddd.Infrastructure.Generators;

public class OtpCodeGenerator
{
    public static async Task<string> Generate(DbSet<OtpEntity> otps, string phoneNumber)
    {
        string otpCode;
        bool isExists;

        do
        {
            otpCode = SecureOtpCode();
            isExists = await otps.AnyAsync(otp => otp.PhoneNumber == phoneNumber && otp.OtpCode == otpCode);
        } while (isExists);

        return otpCode;
    }

    private static string SecureOtpCode()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] randomNumber = new byte[4];
        rng.GetBytes(randomNumber);
        int otpCode = Math.Abs(BitConverter.ToInt32(randomNumber, 0)) % 100000;
        return otpCode.ToString("D5");
    }
}