using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class UnauthorizedResponse<T> : ApiResponse<T>
{
    public UnauthorizedResponse()
        : base(StatusCodes.Status401Unauthorized, success: false,
            error: "شما مجاز به دسترسی به این محتوا نیستید. لطفاً با تیم پشتیبانی تماس بگیرید.")
    {
    }
}
