namespace DetkiProd.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseSwaggerIfDevelopment(this WebApplication webApplication)
    {
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }

        return webApplication;
    }
}
