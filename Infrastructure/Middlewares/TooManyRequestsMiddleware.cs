﻿using System.Text.Json;
using net_core_web_api_clean_ddd.Infrastructure.ExceptionHandlers;

namespace net_core_web_api_clean_ddd.Infrastructure.Middlewares;

public class TooManyRequestsMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            TooManyRequestsResponse<object> response = new();
            string jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
