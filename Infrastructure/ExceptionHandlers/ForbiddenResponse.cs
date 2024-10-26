using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class ForbiddenResponse<T>() : ApiResponse<T>(StatusCodes.Status403Forbidden, success: false,
    error: "شما مجاز به دسترسی به این محتوا نیستید. لطفاً با تیم پشتیبانی تماس بگیرید.");