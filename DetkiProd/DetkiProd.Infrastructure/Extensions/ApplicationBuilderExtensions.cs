using DetkiProd.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DetkiProd.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
