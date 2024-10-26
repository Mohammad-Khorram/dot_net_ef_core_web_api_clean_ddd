using System.Text;

namespace net_core_web_api_clean_ddd.Infrastructure.Generators;

public class HashGenerator
{
    public static string Generate()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder hashBuilder = new();
        Random random = new();
        for (int i = 0; i < 11; i++)
            hashBuilder.Append(chars[random.Next(chars.Length)]);
        return hashBuilder.ToString();
    }
}