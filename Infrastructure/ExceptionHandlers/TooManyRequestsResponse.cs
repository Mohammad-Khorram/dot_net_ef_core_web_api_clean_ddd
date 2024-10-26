using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class TooManyRequestsResponse<T>()
    : ApiResponse<T>(StatusCodes.Status429TooManyRequests, success: false,
        error: "تعداد درخواست\u200cهای شما بیش از حد مجاز است. لطفا کمی بعد دوباره امتحان کنید.");