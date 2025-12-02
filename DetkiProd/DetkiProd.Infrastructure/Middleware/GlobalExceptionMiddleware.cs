using DetkiProd.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace DetkiProd.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (DomainException ex)
        {
            _logger.LogWarning(
                $"Domain exception occurred. " +
                $"ExceptionMessage=[{ex.Message}], " +
                $"ExceptionType=[{ex.GetType()}]");

            await HandleDomainExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            InvalidUserDataException => StatusCodes.Status400BadRequest,
            InvalidProjectDataException => StatusCodes.Status400BadRequest,
            UserNotFoundException => StatusCodes.Status404NotFound,
            UserAlreadyExistsException => StatusCodes.Status409Conflict,
            UserRoleNotFoundException => StatusCodes.Status404NotFound,
            ProjectNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            exception.Message
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            context.Response.StatusCode,
            exception.Message,
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
