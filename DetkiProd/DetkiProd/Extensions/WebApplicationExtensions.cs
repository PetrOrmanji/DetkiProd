namespace DetkiProd.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseSwaggerAnyway(this WebApplication webApplication)
    {
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        return webApplication;
    }

    public static IApplicationBuilder UseCorsPolicy(this WebApplication webApplication)
    {
        webApplication.UseCors("AllowAll");

        return webApplication;
    }
}
