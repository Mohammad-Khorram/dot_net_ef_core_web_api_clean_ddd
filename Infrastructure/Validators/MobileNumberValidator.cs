using System.Text.RegularExpressions;

namespace net_core_web_api_clean_ddd.Infrastructure.Validators;

public class MobileNumberValidator
{
    public static bool IsValid(string mobileNumber)
    {
        return Regex.IsMatch(mobileNumber, @"^09\d{9}$");
    }
}