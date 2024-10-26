using System.Text.Json;
using Microsoft.Extensions.Primitives;
using net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

namespace net_core_web_api_clean_ddd.Infrastructure.Middlewares;

public class SecurityMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public async Task Invoke(HttpContext context)
    {
        string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        // If api-key is missing or token is missing or statusCode is 401
        if (/*authHeader == null || !authHeader.StartsWith("Bearer ") ||*/
            context.Response.StatusCode == StatusCodes.Status401Unauthorized /*||
            !context.Request.Headers.TryGetValue("Api-Key", out StringValues apiKeyHeader)*/)
        {
            UnAuthorizedResponse(context);
            return;
        }

        /*string? apiKeyValue = configuration.GetValue<string>("ApiKey");

        // If api-key is invalid
        if (apiKeyValue == null || !apiKeyValue.Equals(apiKeyHeader))
        {
            UnAuthorizedResponse(context);
            return;
        }*/

        await next(context);
    }

    private static async void UnAuthorizedResponse(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json; charset=utf-8";
        UnauthorizedResponse<object> response = new();
        string jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}