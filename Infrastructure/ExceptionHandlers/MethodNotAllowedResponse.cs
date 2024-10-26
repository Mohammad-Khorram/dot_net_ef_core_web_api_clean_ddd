using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class MethodNotAllowedResponse<T>()
    : ApiResponse<T>(StatusCodes.Status405MethodNotAllowed, success: false,
        error: "درخواست شما معتبر نیست. لطفاً درخواست خود را بررسی کرده و دوباره امتحان کنید.");