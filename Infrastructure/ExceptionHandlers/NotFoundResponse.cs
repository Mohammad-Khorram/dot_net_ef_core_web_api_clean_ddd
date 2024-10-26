using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class NotFoundResponse<T>(string error)
    : ApiResponse<T>(StatusCodes.Status404NotFound, success: false, error: error);