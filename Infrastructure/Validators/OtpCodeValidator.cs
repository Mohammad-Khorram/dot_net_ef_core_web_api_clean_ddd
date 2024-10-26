using System.Text.RegularExpressions;

namespace net_core_web_api_clean_ddd.Infrastructure.Validators;

public class OtpCodeValidator
{
    public static bool IsValid(string otpCode)
    {
        return Regex.IsMatch(otpCode, @"^\d{5}$");
    }
}