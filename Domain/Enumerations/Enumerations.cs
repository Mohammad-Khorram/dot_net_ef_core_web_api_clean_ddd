namespace net_core_web_api_clean_ddd.Domain.Enumerations;

public static class Enumerations
{
    public enum UserRole
    {
        Admin,
        User
    }

    public enum IssuerType
    {
        AdminPanel,
        MobileApp
    }

    public enum AudienceType
    {
        AdminUsers,
        MobileUsers,
    }
}