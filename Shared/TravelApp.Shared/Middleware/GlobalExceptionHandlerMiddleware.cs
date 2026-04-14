using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TravelApp.Shared.Middleware;

/// <summary>
/// ASP.NET Core middleware that catches any unhandled exceptions thrown during request processing
/// and returns a standardized RFC 7807 <c>ProblemDetails</c> JSON response with HTTP 500.
/// Register this middleware early in the pipeline (before <c>UseRouting</c>) so it wraps all subsequent middleware.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GlobalExceptionHandlerMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger used to record unhandled exceptions.</param>
    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware. Passes the request to the next delegate and catches any unhandled exceptions,
    /// delegating error response writing to <see cref="HandleExceptionAsync"/>.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Writes a <c>ProblemDetails</c> JSON response with HTTP 500 to the current response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The unhandled exception that was caught.</param>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "An internal server error occurred.";
        var detail = exception.Message;

        if (exception is TravelApp.Shared.Exceptions.AppException appEx)
        {
            statusCode = (int)appEx.StatusCode;
            title = "Application Error";
        }

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var result = JsonSerializer.Serialize(problemDetails, options);
        return context.Response.WriteAsync(result);
    }
}
