using System.Security.Cryptography;

namespace net_core_web_api_clean_ddd.Infrastructure.Generators;

public class RefreshTokenGenerator
{
    public static string Generate()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}