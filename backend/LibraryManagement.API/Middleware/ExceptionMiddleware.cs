namespace LibraryManagement.API.Middleware;
public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch { 
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; 
            await context.Response.WriteAsJsonAsync(new { message = "Unexpected server error" }); 
        }
    }
}