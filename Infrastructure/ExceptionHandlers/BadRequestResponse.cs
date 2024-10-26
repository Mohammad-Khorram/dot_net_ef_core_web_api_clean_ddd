using net_core_web_api_clean_ddd.Shared;

namespace net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

public class BadRequestResponse<T>(string error)
    : ApiResponse<T>(StatusCodes.Status400BadRequest, success: false, error: error);