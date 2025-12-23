using System.Net;
using System.Text.Json;

namespace ShoppingListAPI.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones
/// Devuelve respuestas JSON consistentes según ARCHITECTURE.md
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            error = "Ocurrió un error interno en el servidor",
            code = context.Response.StatusCode,
            message = exception.Message,
            // Solo incluir stack trace en desarrollo
            details = context.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsDevelopment() 
                ? exception.StackTrace 
                : null
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions)
        );
    }
}

/// <summary>
/// Extensión para registrar el middleware fácilmente
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
