using System.Net;
using System.Text.Json.Serialization;

namespace net_core_web_api_clean_ddd.Shared;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool? Success { get; set; }
    public string? Error { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalPages { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Page { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Limit { get; set; }

    public T? Response { get; set; }

    public ApiResponse(int statusCode = StatusCodes.Status200OK, bool success = true, string? error = null,
        T? response = default)
    {
        StatusCode = statusCode;
        Success = success;
        Error = error;
        Response = response;
    }

    // response with pagination
    public ApiResponse(int? totalPages, int? page, int? limit, int statusCode = StatusCodes.Status200OK,
        bool success = true, string? error = null, T? response = default)
    {
        StatusCode = statusCode;
        Success = success;
        Error = error;
        Response = response;
        TotalPages = totalPages;
        Page = page;
        Limit = limit;
    }
}