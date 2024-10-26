using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class RequestTimeoutResponse<T>()
    : ApiResponse<T>(StatusCodes.Status408RequestTimeout, success: false,
        error: "زمان درخواست شما به اتمام رسید. لطفا\u064b کمی بعد دوباره امتحان کنید.");