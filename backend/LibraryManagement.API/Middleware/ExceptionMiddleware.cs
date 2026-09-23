using System.Net;
using System.Text.Json;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.API.Middleware;

/// <summary>
/// Middleware xử lý lỗi dùng chung: bắt exception nghiệp vụ và trả về JSON
/// với mã HTTP phù hợp, tránh lộ chi tiết stack trace cho client.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            BusinessRuleException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            InvalidCredentialsException => HttpStatusCode.Unauthorized,
            ConflictException => HttpStatusCode.Conflict,
            ForbiddenException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Lỗi không xác định.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            success = false,
            message = statusCode == HttpStatusCode.InternalServerError
                ? "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau."
                : ex.Message
        });

        await context.Response.WriteAsync(payload);
    }
}
